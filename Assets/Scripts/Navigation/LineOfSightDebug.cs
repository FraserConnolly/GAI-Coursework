using com.cyborgAssets.inspectorButtonPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Navigation
{
    public class LineOfSightDebug : MonoBehaviour
    {
        [SerializeField]
        Transform _startTransform;

        [SerializeField]
        Transform _endTransform;

        Vector3 _startPoint, _endPoint;
        bool isDirty = true;

        void Update()
        {
            transform.position = Vector3.zero;

            if (_startTransform == null)
            {
                var go = new GameObject("start");
                go.transform.SetParent(transform);
                go.transform.position = Vector3.zero;
                _startTransform = go.transform;
            }

            if (_endTransform == null)
            {
                var go = new GameObject("end");
                go.transform.SetParent(transform);
                go.transform.position = Vector3.zero;
                _endTransform = go.transform;
            }

            Vector2Int temp = new Vector2Int();

            if (_startPoint != _startTransform.position)
            {
                _startPoint = _startTransform.position;
                temp.x = (int)_startPoint.x;
                temp.y = (int)_startPoint.y;

                if (temp != _startCoordinate)
                {
                    isDirty = true;
                    _startCoordinate = temp;
                }
            }

            if (_endPoint != _endTransform.position)
            {
                _endPoint = _endTransform.position;
                temp.x = (int)_endPoint.x;
                temp.y = (int)_endPoint.y;

                if (temp != _endCoordinate)
                {
                    isDirty = true;
                    _endCoordinate = temp;
                }
            }

            if (isDirty)
            {
                isDirty = false;
                GetLine();
            }
        }

        Vector2Int _startCoordinate;
        Vector2Int _endCoordinate;

        List<Node> _checkedCells = new List<Node>();

        private void GetLine()
        {
            if (_startCoordinate == default(Vector2Int))
            {
                return;
            }

            if (_endCoordinate == default(Vector2Int))
            {
                return;
            }

            var start = Node.GetNodeAtPoint(_startCoordinate, Node.GetAllNodes());
            var end =   Node.GetNodeAtPoint(_endCoordinate, Node.GetAllNodes());

            var e = Node.GetNodesBetweenPoints ( start, end ) ;

            _checkedCells.Clear();
            foreach (var item in e)
            {
                _checkedCells.Add(item);
            }

            Debug.Log($"Getting line of sight between ({_startCoordinate.x}, {_startCoordinate.y}) and ({_endCoordinate.x}, {_endCoordinate.y}). Points = {_checkedCells?.Count ?? 0}");
        }

        private void OnDrawGizmos()
        {
            if (_checkedCells == null || !_checkedCells.Any())
            {
                return;
            }

            for (int i = 0; i < _checkedCells.Count; i++)
            {
                Node node = _checkedCells[i];

                // This is because the world coordinate of a node is in the bottom left
                // of the grid square that is rendered for the node.
                const float gridOffset = 0.5f;

                Vector3 lineStart = new Vector3(node.Coordinate.x + gridOffset, node.Coordinate.y + gridOffset, 0f);

                Gizmos.color = node.Navigable ? Color.white : Color.red;
                Gizmos.DrawSphere(lineStart, 0.4f);
            }
        }
    }
}
