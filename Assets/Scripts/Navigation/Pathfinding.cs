using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class Pathfinding
    {
        public static IReadOnlyList<Node> GetPath ( Vector2Int startPoint, Vector2Int endPoint, out int cost )
        {
            cost = -1;
            
            // check if either the start or end point is not navigable.
            if ( ! GameData.Instance.Map.IsNavigatable(startPoint.x, startPoint.y) 
              || ! GameData.Instance.Map.IsNavigatable(endPoint.x,   endPoint.y  ) )
            {
                return null;
            }

            var nodes = Node.GetAllNodes();

            Node startNode, endNode;

            startNode = Node.GetNodeAtPoint(startPoint, nodes);
            endNode   = Node.GetNodeAtPoint(endPoint,   nodes);

            if ( startNode == null )
            {
                Debug.Log("Start node not found");
                return null;
            }

            if ( endNode == null)
            {
                Debug.Log("End node not found");
                return null;
            }
            
            var path = AStar ( startNode, endNode, EuclideanDistanceHeuristic);

            cost = endNode.g;
            return path;
        }

        private class PriorityQueueOfNodes
        {
            public PriorityQueueOfNodes(Func<Node, int> orderingFunction)
            {
                OrderingFunction = orderingFunction;
            }

            List<Node> storage = new List<Node>(75);

            public int Count => storage.Count;

            public Func<Node, int> OrderingFunction { get; }

            public void Push(Node item)
            {
                storage.Add(item);
                storage = storage.OrderBy(OrderingFunction).ToList();
            }

            public Node Pop()
            {
                if (storage.Count == 0)
                {
                    return null;
                }

                Node n = storage.First();
                storage.Remove(n);
                return n;
            }

            public bool OnList(Node n)
            {
                return storage.Contains(n);
            }

        }

        private static List<Node> AStar(Node startNode, Node endNode, Func<int, int, int, int, int> heuristicFunction)
        {
            PriorityQueueOfNodes openList = new PriorityQueueOfNodes(node => node.f);
            List<Node> closedList = new List<Node>();

            openList.Push(startNode);    // This is a priority queue
            int visitOrder = 0; // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not be used in production code

            while (openList.Count > 0)
            {
                Node currentNode = openList.Pop();
                closedList.Add(currentNode); //currentNode.onClosedList = true;

                if (currentNode == endNode)
                {
                    return GetFoundPath(endNode);
                }

                for (int neighbourIndex = 0; neighbourIndex < currentNode.Neighbors.Count(); neighbourIndex++)
                {
                    Node currentNeighbour = currentNode.Neighbors[neighbourIndex];

                    if ( closedList.Contains(currentNeighbour) )
                    {
                        continue;
                    }

                    var g = currentNode.g + currentNode.neighborCosts[neighbourIndex]; //cost to childNode from currentNode
                    var h = heuristicFunction(currentNeighbour.Coordinate.x, currentNeighbour.Coordinate.y, endNode.Coordinate.x, endNode.Coordinate.y);
                    if (g + h <= currentNeighbour.f || !openList.OnList(currentNeighbour))
                    {
                        currentNeighbour.Parent = currentNode;
                        currentNeighbour.g = g;
                        currentNeighbour.h = h;
                    }

                    if (!openList.OnList(currentNeighbour))
                    {
                        openList.Push(currentNeighbour);
                    }
                }
            }

            Debug.LogWarning("Failed to find a route using Dijkstra");

            // No path has been found
            return GetFoundPath(null);
        }

        private static int ChebyshevDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
        {
            var distance = Mathf.Max(Mathf.Abs(targetY - currentY), Mathf.Abs(targetX - currentX));
            return distance * 10; // multiply by 10 as the grid world is 10 times larger than the coordinate system.
        }

        private static int EuclideanDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
        {
            var distance = MathF.Sqrt(
                ((targetX - currentX) * (targetX - currentX)) +
                ((targetY - currentY) * (targetY - currentY))
                );

            return (int)distance * 10; // multiply by 10 as the grid world is 10 times larger than the coordinate system.
        }

        private static int ManhattanDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
        {
            var distance = MathF.Abs(currentX - targetX) + MathF.Abs(currentY - targetY);
            return (int)distance * 10; // multiply by 10 as the grid world is 10 times larger than the coordinate system.
        }

        private static List<Node> GetFoundPath(Node endNode)
        {
            List<Node> foundPath = new List<Node>();
            if (endNode != null)
            {
                foundPath.Add(endNode);

                while (endNode.Parent != null)
                {
                    foundPath.Add(endNode.Parent);
                    endNode = endNode.Parent;
                }

                // Reverse the path so the start node is at index 0
                foundPath.Reverse();
            }

            return foundPath;
        }

    }
}
