using GCU.FraserConnolly.AI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{

    public class PathFollow : SteeringBehaviour, IWeightable
    {
        [SerializeField, Range(0f, 1f)]
        private float _weight = 1f;
        public float Weight => _weight;

        private IReadOnlyList<Node> _path;
        private int _nextNodeIndex = -1;

        [SerializeField]
        private float allowedDistanceToTarget = 5f;

        public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
        {
            if ( _nextNodeIndex < 0 )
            {
                steeringVelocity = Vector3.zero;
                desiredVelocity = Vector3.zero;
                return Vector3.zero;
            }

            Vector3? targetCoordinate = GetNextWayPointCoordinate();

            if ( ! targetCoordinate.HasValue )
            {
                clearPath();
                return Vector3.zero;
            }

            // seek to waypoint coordinate

            // Get the desired velocity for seek and limit to maxSpeed
            desiredVelocity = Vector3.Normalize(targetCoordinate.Value - transform.position) * SteeringAgent.MaxCurrentSpeed;

            // Calculate steering velocity
            steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
            return steeringVelocity;
        }

        /// <summary>
        /// Returns the 3D coordinate of the next waypoint in the path that the agent should seek to.
        /// </summary>
        /// <returns>Will return null if there are no more way points to reach.</returns>
        private Vector3? GetNextWayPointCoordinate()
        {
            if (_nextNodeIndex >= _path.Count || _nextNodeIndex < 0)
            {
                return null;
            }

            Node currentNode = _path[_nextNodeIndex]; 
            Vector3 targetCoordinate;

            targetCoordinate = new Vector3(currentNode.Coordinate.x + 0.5f, currentNode.Coordinate.y + 0.5f, transform.position.z);

            float targetDistance = (targetCoordinate - transform.position).sqrMagnitude;

            if (targetDistance < (allowedDistanceToTarget * allowedDistanceToTarget))
            {
                if ( _nextNodeIndex + 1 >= _path.Count )
                {
                    // there are no more nodes to move on to.
                    return null;
                }
                    
                Node nextNode = _path[_nextNodeIndex + 1];

                Vector2Int currentLocation = new Vector2Int((int)transform.position.x, (int)transform.position.y);

                Node transformNode = Node.GetNodeAtPoint(currentLocation, Node.GetAllNodes());

                // check for line of sight from current location to the proposed waypoint.
                if (Node.HasLineOfSightBetweenNodes(transformNode, nextNode))
                {
                    // this node is close and can seen so start moving towards that node.
                    _nextNodeIndex++;
                    targetCoordinate = new Vector3(nextNode.Coordinate.x + 0.5f, nextNode.Coordinate.y + 0.5f, transform.position.z);
                }
            }

            return targetCoordinate;
        }

        public void setPath (IReadOnlyList<Node> path)
        {
            if ( path == null || ! path.Any( ) )
            {
                Debug.LogWarning("Can not set an empty or null path.");
                return;
            }

            _path = path;
            _nextNodeIndex = 0;

            // find the first waypoint which has line of sight to our current location and where the distance to the next way point is larger than this waypoint.
            // this may not be the closet waypoint but it is an optimised solution such that not all waypoints need to be inspected.

            Vector2Int currentMapCell = new Vector2Int( (int) transform.position.x, (int) transform.position.y );
            int smallestDistance = (_path[0].Coordinate - currentMapCell).sqrMagnitude;

            for (int i = 0; i < _path.Count; i++)
            {
                var node = _path[i];
                int distance = (node.Coordinate - currentMapCell).sqrMagnitude;
                if ( distance < smallestDistance )
                {
                    if ( Node.HasLineOfSightBetweenNodes ( Node.GetNodeAtPoint( currentMapCell, Node.GetAllNodes() ), node))
                    {
                        smallestDistance = distance;
                        _nextNodeIndex = i;
                    }
                }
                else
                {
                    // this node is further away than the best option we have so far, so stop checking.
                    break;
                }
            }
        }

        public void clearPath()
        {
            _path = null;
            _nextNodeIndex = -1;
            steeringVelocity = Vector3.zero;
            desiredVelocity = Vector3.zero;
        }

        public bool isFollowingPath => _path != null;

        private void OnDrawGizmosSelected()
        {
            if (_path == null || !_path.Any())
            {
                return;
            }

            if ( ! ShowDebugLines )
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

                Gizmos.color = i < _nextNodeIndex ? Color.red : Color.yellow;

                Vector3 lineStart = new Vector3(node.Coordinate.x + gridOffset, node.Coordinate.y + gridOffset, 0f);
                Vector3 lineEnd = new Vector3(nextNode.Coordinate.x + gridOffset, nextNode.Coordinate.y + gridOffset, 0f);

                Gizmos.DrawLine(lineStart, lineEnd);
                Gizmos.DrawSphere(lineEnd, 0.4f);
            }
        }

    }
}
