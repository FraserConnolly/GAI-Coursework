using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class NodeDebug : MonoBehaviour
    {
        public static Map getMap()
        {
            var mapTester = FindAnyObjectByType<MapDebug>();
            if (mapTester != null)
            {
                return mapTester.map;
            }
            else
            {
                return GameData.Instance?.Map ?? null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            IReadOnlyList<Node> nodes;

            //var mapTester = FindAnyObjectByType<MapDebug>();
            //if (mapTester != null)
            //{
            //    nodes = Node.GetAllNodes(mapTester.map);
            //}
            //else
            //{
            //    nodes = Node.GetAllNodes();
            //}

            nodes = Node.GetAllNodes(getMap());

            // Convert the local coordinate values into world
            // coordinates for the matrix transformation.
            Gizmos.matrix = transform.localToWorldMatrix;

            foreach ( Node node in nodes ) 
            {
                for ( int i = 0; i < node.Neighbors.Count; i++ )
                {
                    Node neighbours = node.Neighbors[i];

                    // This is because the world coordinate of a node is in the bottom left
                    // of the grid square that is rendered for the node.
                    const float gridOffset = 0.5f;

                    Vector3 lineStart = new Vector3(0f, node.Coordinate.x + gridOffset, node.Coordinate.y + gridOffset);
                    Vector3 lineEnd = new Vector3(0f, neighbours.Coordinate.x + gridOffset, neighbours.Coordinate.y + gridOffset);

                    switch (node.neighborCosts[i])
                    {
                        case 20: // grass to grass
                        case 2*14: // grass to grass diagonal
                            Gizmos.color = Color.green;
                            break;
                        case 80: // water to water
                        case 8*14: // water to water diagonal
                            Gizmos.color = Color.blue;
                            break;
                        case 40: // mud to mud
                        case 4*14: // mud to mud diagonal
                            Gizmos.color = Color.gray;
                            break;
                        case 50: // grass to water
                        case 5*14: // grass to water diagonal
                            Gizmos.color = Color.cyan;
                            break;
                        case 30: // grass to mud
                        case 3*14: // grass to mud diagonal
                            Gizmos.color = Color.red;
                            break;
                        case 60: // mud to water
                        case 6*14: // mud to water diagonal
                            Gizmos.color = Color.yellow;
                            break;
                        default:
                            Gizmos.color = Color.black;
                            break;
                    }

                    Gizmos.DrawLine(lineStart, lineEnd);
                }
            }
        }
    }
}
