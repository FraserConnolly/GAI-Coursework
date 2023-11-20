using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    internal class Node
    {
        public Vector2Int Coordinate { get; private set; }
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

        private static List<Node> s_Nodes = new List<Node>();

        public Node(int x, int y)
        {
            Coordinate = new Vector2Int(x, y);
            Navigable = GameData.Instance.Map.IsNavigatable(x, y);

            Neighbours
        }

        private static void BuildNodes ()
        {
            if ( s_Nodes.Any() )
            {
                return;
            }

            Map map = GameData.Instance.Map;

            var mapData = map.GetMapData();

            for ( int i = 0; i < Map.MapWidth ; i++ ) 
            {
                for (int j = 0; j < Map.MapHeight; j++)
                {
                    Node n = new Node(i, j);
                    s_Nodes.Add(n);
                }
            }
        }

        public static IReadOnlyList<Node> GetCopyOfAllNodes()
        {

        }

    }
}
