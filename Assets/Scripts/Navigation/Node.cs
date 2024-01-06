using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly
{
    public class Node
    {
        public Vector2Int Coordinate { get; private set; }
        public Map.Terrain Terrain { get; private set; }
        public bool Navigable { get; private set; }

        private Node[] neighbours;
        private Node[] navigableNeighbours;

        /// <summary>
        /// Navigable Neighbours
        /// </summary>
        public IReadOnlyList<Node> Neighbours => navigableNeighbours;
        public int[] neighbourCosts;

        public int islandID { get; private set; } = 0;

        public Node Parent { get; set; }

        /// <summary>
        /// Final cost that is a summation of g+ h (used in A* and Dijkstra only)
        /// </summary>
        public int f => g + h;

        /// <summary>
        /// Goal cost that is the current cost to get to this node (used in A* and Dijkstra only)
        /// </summary>
        public int g { get; set; }

        /// <summary>
        /// Heuristic cost that is the best guess to get to the goal bode from here (used in A* only)
        /// </summary>
        public int h { get; set; }

        public Node North { get; private set; }
        public Node South { get; private set; }
        public Node East { get; private set; }
        public Node West { get; private set; }

        private static Map s_map = null;
        private static List<Island> s_Islands = new List<Island>();
        private static Node[] s_Nodes = null;

        public Node(int x, int y)
        {
            Coordinate = new Vector2Int(x, y);
            Navigable = s_map.IsNavigatable(x, y);
            Terrain = s_map.GetTerrainAt(x, y);
        }

        private Node GetRelativeNode(int x, int y)
        {
            x += Coordinate.x;
            y += Coordinate.y;

            if (x < 0 || x >= Map.MapWidth)
            {
                return null;
            }

            if (y < 0 || y >= Map.MapHeight)
            {
                return null;
            }

            return GetNodeAtPoint(x, y, s_Nodes);
        }

        /// <summary>
        /// Should be called on scene load.
        /// </summary>
        public static void ClearNodes()
        {
            s_Nodes = null;
            s_Islands.Clear();
        }

        private static void BuildNodes()
        {
            if (s_Nodes != null)
            {
                foreach (var node in s_Nodes)
                {
                    node.clearTempData();
                }
                return;
            }

            if (s_map == null)
            {
                return;
            }

            Debug.Log("Building nodes");

            s_Nodes = new Node[ Map.MapSize ];

            for ( int i = 0; i < Map.MapWidth ; i++ )
            {
                for (int j = 0; j < Map.MapHeight; j++)
                {
                    int nodeIndex = i + (Map.MapWidth * j);
                    Node n = new Node(i, j);
                    s_Nodes[nodeIndex] = n;
                }
            }

            EstablishNodeConnections();
            SetIslandID();
        }

        /// <summary>
        /// Inspired by https://gamedev.stackexchange.com/questions/169483/finding-islands-from-array
        /// </summary>
        private static void SetIslandID()
        {
            Queue<Node> queue = new Queue<Node>();
            int islandID = 0;

            foreach (var node in s_Nodes)
            {
                Map.Terrain terrain = node.Terrain;

                if (node.islandID != 0)
                {
                    continue;
                }

                islandID++;
                var island = new Island(islandID, terrain);
                var size = 0;

                node.islandID = islandID;
                queue.Enqueue(node);

                while (queue.Any())
                {
                    size++;
                    var n = queue.Dequeue();

                    foreach (var neighbour in n.neighbours)
                    {
                        if (neighbour == null)
                        {
                            continue;
                        }

                        if (neighbour.Terrain == terrain && neighbour.islandID == 0)
                        {
                            neighbour.islandID = islandID;
                            island.Nodes.Add(neighbour);
                            queue.Enqueue(neighbour);
                        }
                    }
                }

                s_Islands.Add(island);
            }
        }

        private static void EstablishNodeConnections()
        {
            for (int nodeY = 0; nodeY < Map.MapHeight; ++nodeY)
            {
                for (int nodeX = 0; nodeX < Map.MapWidth; ++nodeX)
                {
                    int nodeIndex = nodeX + (Map.MapWidth * nodeY);
                    Node node = s_Nodes[nodeIndex];

                    node.North = node.GetRelativeNode(0, +1);
                    node.South = node.GetRelativeNode(0, -1);
                    node.East = node.GetRelativeNode(+1, 0);
                    node.West = node.GetRelativeNode(-1, 0);

                    node.neighbours = new Node[]
                    {
                        node.GetRelativeNode(0,   1 ), // N
                        node.GetRelativeNode(1,   1 ), // NE
                        node.GetRelativeNode(1,   0),  // E 
                        node.GetRelativeNode(1,  -1 ), // SE
                        node.GetRelativeNode(0,  -1 ), // S
                        node.GetRelativeNode(-1, -1 ), // SW
                        node.GetRelativeNode(-1,  0 ), // W
                        node.GetRelativeNode(-1,  1 ), // NW
                    };

                    if (!node.Navigable)
                    {
                        node.navigableNeighbours = new Node[0];
                        node.neighbourCosts = new int[0];
                        continue;
                    }

                    // Count the number of navigable neighbours
                    int connectedNodesCount = CountNavigableNeighbours(nodeY, nodeX);

                    node.navigableNeighbours = new Node[connectedNodesCount];
                    node.neighbourCosts = new int[connectedNodesCount];

                    int connectedNodesIndex = 0;
                    for (int neighbourY = nodeY - 1; neighbourY <= nodeY + 1; ++neighbourY)
                    {
                        if (neighbourY < 0 || neighbourY >= Map.MapHeight)
                        {
                            continue;
                        }

                        for (int neighbourX = nodeX - 1; neighbourX <= nodeX + 1; ++neighbourX)
                        {
                            if (neighbourX < 0 ||
                                neighbourX >= Map.MapWidth ||
                                (neighbourX == nodeX && neighbourY == nodeY) ||
                                !s_map.IsNavigatable(neighbourX, neighbourY))
                            {
                                continue;
                            }
                            var neighbourNode = s_Nodes[neighbourX + (neighbourY * Map.MapWidth)];
                            node.navigableNeighbours[connectedNodesIndex] = neighbourNode ;
                            node.neighbourCosts[connectedNodesIndex] = CalculateTerrainCost(node.Terrain , neighbourNode.Terrain, CalculateTravelCost(nodeX, nodeY, neighbourX, neighbourY));
                            ++connectedNodesIndex;
                        }
                    }
                }
            }
        }

        static Node()
        {
            GameManager.OnSceneReload += OnSceneReload;
        }

        private static int CalculateTerrainCost ( Map.Terrain firstNodeTerrain, Map.Terrain secondNodeTerrain, int movementCost )
        {
            int firstNodeTerrainCost = GetTerrainCost( firstNodeTerrain );
            int secondNodeTerrainCost = GetTerrainCost( secondNodeTerrain );

            return ( firstNodeTerrainCost + secondNodeTerrainCost ) * movementCost;
        }

        private static int GetTerrainCost(Map.Terrain firstNodeTerrain)
        {
            const int GRASS_COST = 1;
            const int MUD_COST   = 2;
            const int WATER_COST = 4;

            switch (firstNodeTerrain)
            {
                case Map.Terrain.Water:
                    return WATER_COST;
                case Map.Terrain.Mud:
                    return MUD_COST;
                case Map.Terrain.Grass:
                default:
                    return GRASS_COST;
            }
        }

        private static int CalculateTravelCost(int firstNodeX, int firstNodeY, int secondNodeX, int secondNodeY)
        {
            const int ORTHOGONAL_COST = 10;
            const int DIAGONAL_COST = 14;

            int xCost = Mathf.Abs(secondNodeX - firstNodeX);
            int yCost = Mathf.Abs(secondNodeY - firstNodeY);

            if ((xCost + yCost) < 2)
            {
                if (xCost > 0)
                {
                    // x cost 
                    return ORTHOGONAL_COST;
                }

                // y cost 
                return ORTHOGONAL_COST;
            }
            return DIAGONAL_COST;
        }

        private static int CountNavigableNeighbours(int nodeY, int nodeX)
        {
            int connectedNodesCount = 0;

            for (int neighbourY = nodeY - 1; neighbourY <= nodeY + 1; ++neighbourY)
            {
                if (neighbourY < 0 || neighbourY >= Map.MapHeight)
                {
                    continue;
                }

                for (int neighbourX = nodeX - 1; neighbourX <= nodeX + 1; ++neighbourX)
                {
                    if (neighbourX < 0 ||
                        neighbourX >= Map.MapWidth ||
                        (neighbourX == nodeX && neighbourY == nodeY) ||
                        !s_map.IsNavigatable(neighbourX, neighbourY))
                    {
                        continue;
                    }

                    ++connectedNodesCount;
                }
            }

            return connectedNodesCount;
        }

        public static IReadOnlyList<Node> GetAllNodes(Map map = null)
        {
            if (map != null && Node.s_map != map)
            {
                ClearNodes();
                Node.s_map = map;
            }

            if (map == null && Node.s_map == null)
            {
                // use GameData Map
                Node.s_map = GameData.Instance?.Map ?? null;
            }

            BuildNodes();

            return s_Nodes ?? Array.Empty<Node>();
        }

        public static IReadOnlyCollection<Island> GetIsland(Map.Terrain terrain)
        {
            if (s_Islands == null && s_map == null)
            {
                return null;
            }

            if (s_Islands == null)
            {
                BuildNodes();
            }

            return s_Islands.Where(t => t.Terrain == terrain).ToList();
        }

        public static Node GetNodeAtPoint(Vector2Int point, IReadOnlyList<Node> nodes)
        {
            return nodes.Where(n => n.Coordinate == point).FirstOrDefault();
        }

        public static Node GetNodeAtPoint(int x, int y, IReadOnlyList<Node> nodes)
        {
            var index = x + y * Map.MapWidth;

            if (index < 0 || index >= nodes.Count)
            {
                return null;
            }

            return nodes[index];
            //return nodes.Where(n => n.Coordinate.x == x && n.Coordinate.y == y).FirstOrDefault();
        }

        public static IEnumerable<Node> GetNodesBetweenPoints ( Node start, Node end )
        {
            var e = BresLine.GetBresLinePoints( new Point( start.Coordinate.x, start.Coordinate.y), new Point( end.Coordinate.x, end.Coordinate.y));

            foreach (var point in e)
            {
                yield return GetNodeAtPoint(new Vector2Int(point.X, point.Y), s_Nodes);
            }
        }

        public static bool HasLineOfSightBetweenNodes ( Node start, Node end )
        {
            if ( start.Coordinate == end.Coordinate )
            {
                return true;
            }

            foreach (var node in GetNodesBetweenPoints(start, end))
            {
                if ( ! node.Navigable )
                {
                    return false;
                }
            }

            return true;
        }

        private static void OnSceneReload()
        {
            ClearNodes();
        }

        public void clearTempData()
        {
            g = 0;
            h = 0;
            Parent = null;
        }
    }

    public class Island
    {
        public Island(int ID, Map.Terrain terrain)
        {
            this.ID = ID;
            Terrain = terrain;
            Nodes = new List<Node>();
        }

        public int ID { get; private set; }
        public Map.Terrain Terrain { get; private set; }
        public int Size => Nodes.Count;
        public List<Node> Nodes { get; private set; }
    }

    /// <summary>
    /// From:
    /// https://www.codeproject.com/Articles/30686/Bresenham-s-Line-Algorithm-Revisited
    /// </summary>
    public static class BresLine
    {

        public static IEnumerable<Point> GetBresLinePoints ( Point start, Point end )
        {
            if ( start.X > end.X )
            {
                // swap X
                int t = start.X;
                start.X = end.X;
                end.X = t;
            }

            if (start.Y > end.Y)
            {
                // swap X
                int t = start.Y;
                start.Y = end.Y;
                end.Y = t;
            }

            return BresLineOrig( start, end );
        }


        /// <summary>
        /// Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
        /// * where x0 less than x1 and y0 less than y1
        ///   AND line is less steep than it is wide (dx less than dy)
        /// </summary>
        private static IEnumerable<Point> BresLineOrig(Point begin, Point end)
        {
            Point nextPoint = begin;
            int deltax = end.X - begin.X;
            int deltay = end.Y - begin.Y;
            int error = deltax / 2;
            int ystep = 1;

            if (end.Y < begin.Y)
            {
                ystep = -1;
            }

            while (nextPoint.X < end.X)
            {
                if (nextPoint != begin) yield return nextPoint;
                nextPoint.X++;

                error -= deltay;
                if (error < 0)
                {
                    nextPoint.Y += ystep;
                    error += deltax;
                }
            }
        }
    }
}