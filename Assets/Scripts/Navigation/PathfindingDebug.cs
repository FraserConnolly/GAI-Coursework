using com.cyborgAssets.inspectorButtonPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class PathfindingDebug : MonoBehaviour
    {
        [SerializeField]
        Transform _startTransform;

        [SerializeField]
        Transform _endTransform;

        Vector3 _startPoint, _endPoint;
        bool isDirty = true;

        void Update ()
        {
            if (_startTransform == null)
            {
                var go = new GameObject("start");
                go.transform.SetParent(transform);
                _startTransform = go.transform;
            }

            if (_endTransform == null)
            {
                var go = new GameObject("end");
                go.transform.SetParent(transform);
                _endTransform = go.transform;
            }

            if ( _startPoint != _startTransform.position )
            {
                _startPoint = _startTransform.position;
                isDirty = true;
                _startNode.x = (int)_startPoint.x;
                _startNode.y = (int)_startPoint.y;
            }

            if (_endPoint != _endTransform.position)
            {
                _endPoint = _endTransform.position;
                isDirty = true;
                _endNode.x = (int)_endPoint.x;
                _endNode.y = (int)_endPoint.y;
            }

            if ( isDirty )
            {
                isDirty = false;
                GetPath();
            }
        }

        Vector2Int _startNode;
        Vector2Int _endNode;

        IReadOnlyList<Node> _path = null;
        int _cost = 0;

        [ProButton]
        private void GetPath()
        {
            if (_startNode == default(Vector2Int))
            {
                return;
            }

            if (_endNode == default(Vector2Int))
            {
                return;
            }

            _path = Pathfinding.GetPath(_startNode, _endNode, out _cost);
        }

        private void OnDrawGizmos()
        {
            if ( _path == null || ! _path.Any( ) ) 
            { 
                return;
            }

            for (int i = 0; i < _path.Count - 1; i++)
            {
                Node node = _path[i];
                Node nextNode = _path[i + 1];

                // This is because the world coordinate of a node is in the bottom left
                // of the grid square that is rendered for the node.
                const float gridOffset = 0.5f;

                Vector3 lineStart = new Vector3(node.Coordinate.x + gridOffset, node.Coordinate.y + gridOffset, 0f);
                Vector3 lineEnd = new Vector3(nextNode.Coordinate.x + gridOffset, nextNode.Coordinate.y + gridOffset, 0f);

                //switch (node.neighbourCosts[i])
                //{
                //    case 20: // grass to grass
                //    case 2 * 14: // grass to grass diagonal
                //        Gizmos.color = Color.green;
                //        break;
                //    case 80: // water to water
                //    case 8 * 14: // water to water diagonal
                //        Gizmos.color = Color.blue;
                //        break;
                //    case 40: // mud to mud
                //    case 4 * 14: // mud to mud diagonal
                //        Gizmos.color = Color.gray;
                //        break;
                //    case 50: // grass to water
                //    case 5 * 14: // grass to water diagonal
                //        Gizmos.color = Color.cyan;
                //        break;
                //    case 30: // grass to mud
                //    case 3 * 14: // grass to mud diagonal
                //        Gizmos.color = Color.red;
                //        break;
                //    case 60: // mud to water
                //    case 6 * 14: // mud to water diagonal
                //        Gizmos.color = Color.yellow;
                //        break;
                //    default:
                //        Gizmos.color = Color.black;
                //        break;
                //}
                Gizmos.DrawLine(lineStart, lineEnd);
            }
        }
    }
}
