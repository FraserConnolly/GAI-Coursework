using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class Node
    {
        public Vector2Int Coordinate { get; private set; }
        public Map.Terrain Terrain { get; private set; }
        public bool Navigable { get; private set; }

        private Node[] neighbours;
        public IReadOnlyList<Node> Neighbours => neighbours;
        public int[] neighbourCosts;

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

        private static Node[] s_Nodes = null;

        public Node(int x, int y)
        {
            Coordinate = new Vector2Int(x, y);
            Navigable = GameData.Instance.Map.IsNavigatable(x, y);
            Terrain = GameData.Instance.Map.GetTerrainAt(x, y);
        }

        /// <summary>
        /// Should be called on scene load.
        /// </summary>
        public static void ClearNodes()
        {
            s_Nodes = null;
        }

        private static void BuildNodes ()
        {
            // temp disabled for debugging.
            //if ( s_Nodes != null )
            //{
            //    return;
            //}

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
        }

        private static void EstablishNodeConnections()
        {
                for (int nodeY = 0; nodeY < Map.MapHeight; ++nodeY)
                {
                    for (int nodeX = 0; nodeX < Map.MapWidth; ++nodeX)
                {
                    int nodeIndex = nodeX + (Map.MapWidth * nodeY);
                    Node node = s_Nodes[nodeIndex];

                    if (!node.Navigable)
                    {
                        node.neighbours = new Node[0];
                        node.neighbourCosts = new int[0];
                        continue;
                    }

                    // Count the number of navigable neighbours
                    int connectedNodesCount = CountNavigableNeighbours(nodeY, nodeX);

                    node.neighbours = new Node[connectedNodesCount];
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
                                ! GameData.Instance.Map.IsNavigatable(neighbourX, neighbourY))
                            {
                                continue;
                            }
                            var neighbourNode = s_Nodes[neighbourX + (neighbourY * Map.MapWidth)];
                            node.neighbours[connectedNodesIndex] = neighbourNode ;
                            node.neighbourCosts[connectedNodesIndex] = CalculateTerrainCost(node.Terrain ,neighbourNode.Terrain, CalculateTravelCost(nodeX, nodeY, neighbourX, neighbourY));
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
                        ! GameData.Instance.Map.IsNavigatable(neighbourX, neighbourY))
                    {
                        continue;
                    }

                    ++connectedNodesCount;
                }
            }

            return connectedNodesCount;
        }

        public static IReadOnlyList<Node> GetCopyOfAllNodes()
        {
            BuildNodes();

            // to do - check that this is a copy of the nodes list
            return s_Nodes.ToList();
        }

        public static Node GetNodeAtPoint ( Vector2Int point, IReadOnlyList<Node> nodes )
        {
            //   return nodes.Where( n => n.Coordinate == point ).FirstOrDefault();

            foreach (var item in nodes)
            {
                if ( item.Coordinate.x == point.x && item.Coordinate.y == point.y )
                {
                    return item;
                }
            }

            return null;
        }

        private static void OnSceneReload()
        {
            ClearNodes();
        }

    }
}
