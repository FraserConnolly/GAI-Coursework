using GCU.FraserConnolly.AI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{

    public class TreeAvoidance : SteeringBehaviour, IWeightable
    {
        [SerializeField, Range(0f, 1f)]
        private float _weight = 1f;
        public float Weight => _weight;

        [SerializeField]
        private float sensorDistance = 2f;
        
        [SerializeField]
        private float sensorAngle = 20f;

        private class feeler
        {
            public Vector3 feelerDirection;
            public float feelerAngle;
            public float avoidanceAngle;
            public int feelerLength = 2; // in grid units
            public bool isTriggered = false;
        }

        private feeler[] feelers;

        private void Awake()
        {
            feelers = new feeler[] {
                new feeler () { feelerAngle = 0, avoidanceAngle = sensorAngle, feelerLength = (int)sensorDistance },
                new feeler () { feelerAngle = sensorAngle, avoidanceAngle = -sensorAngle, feelerLength = (int)sensorDistance },
                new feeler () { feelerAngle = -sensorAngle, avoidanceAngle = sensorAngle, feelerLength = (int)sensorDistance },
            };
        }

        public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
        {
            desiredVelocity = Vector3.zero;

            foreach (var feeler in feelers)
            {
                feeler.feelerLength = (int)sensorDistance;
                feeler.feelerDirection = Quaternion.Euler(0f, 0f, feeler.feelerAngle) * transform.up;
                Vector3 avoidance = Quaternion.Euler(0f, 0f, feeler.avoidanceAngle) * transform.up;

                if (CheckFeeler(feeler.feelerDirection, feeler.feelerLength))
                {
                    // adjust direction
                    desiredVelocity += avoidance;
                    feeler.isTriggered = true;
                }
                else
                {
                    feeler.isTriggered = false;
                }
            }

            if (desiredVelocity != Vector3.zero)
            {

                desiredVelocity.Normalize();
                desiredVelocity *= SteeringAgent.MaxCurrentSpeed;
                
                // Calculate steering velocity
                steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
                return steeringVelocity;
            }


            desiredVelocity = Vector3.zero;
            steeringVelocity = Vector3.zero;
            return Vector3.zero;
        }

        private bool CheckFeeler ( Vector3 feelerDirection, int feelerDistance )
        {
            feelerDirection.Normalize();

            for (int length = 1; length <= feelerDistance; length++)
            {
                Vector3 pointToCheck = transform.position + ( feelerDirection * length );

                Vector2Int mapCell = new Vector2Int((int)pointToCheck.x, (int)pointToCheck.y);

                if ( mapCell.x < 0 || mapCell.y < 0 || mapCell.x >= Map.MapWidth || mapCell.y >= Map.MapHeight )
                {
                    // not a valid map cell to check.
                    continue;
                }

                if (!GameData.Instance.Map.IsNavigatable(mapCell.x, mapCell.y))
                {
                    return true;
                }
            }

            return false;
        }


        private void OnDrawGizmosSelected()
        {
            if (!ShowDebugLines)
            {
                return;
            }

            foreach (var feeler in feelers)
            {
                Gizmos.color = feeler.isTriggered ? Color.red : Color.white;
                Gizmos.DrawLine(transform.position, transform.position + (feeler.feelerDirection * feeler.feelerLength));
            }
        }

    }
}
