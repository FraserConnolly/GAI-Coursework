using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class NodeDebug : MonoBehaviour
    {
        IReadOnlyList<Node> _nodes = null;

        private void Start()
        {
            _nodes = Node.GetCopyOfAllNodes();
        }

        private void OnDrawGizmosSelected()
        {
            if ( _nodes == null ) 
            { 
                return;
            }

            foreach ( Node node in _nodes ) 
            {
                for ( int i = 0; i < node.Neighbours.Count; i++ )
                {
                    Node neighbours = node.Neighbours[i];

                    // This is because the world world coordinate of a node is in the bottom left
                    // of the grid square that is rendered for the node.
                    const float gridOffset = 0.5f;

                    Vector3 lineStart = new Vector3(node.Coordinate.x + gridOffset, node.Coordinate.y + gridOffset, 0f);
                    Vector3 lineEnd = new Vector3(neighbours.Coordinate.x + gridOffset, neighbours.Coordinate.y + gridOffset, 0f);

                    switch (node.neighbourCosts[i])
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
