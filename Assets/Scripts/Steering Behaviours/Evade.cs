using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{
    /// <summary>
    /// An agent evades oncoming fire by moving around a point.
    /// This behaviour utilises the oscillation caused by seek to make an agent continually move at a point 
    /// without causing them to move out of their attack range.
    /// </summary>
    public class Evade : SteeringBehaviour, IWeightable
    {

        [SerializeField, Range(0f, 1f)]
        private float _weight = 1f;
        public float Weight => _weight;

        Vector3? _targetPoint;

        public void SetTarget(Vector3 target, bool autoEnable = true)
        {
            target.z = 0;
            _targetPoint = target;

            if (autoEnable)
            {
                enabled = true;
                _weight = 1f;
            }
        }

        public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
        {
            if (_targetPoint == null)
            {
                // disable this behaviour if there is no target to seek towards.
                //enabled = false;
                return Vector3.zero;
            }

            // Get the desired velocity for seek and limit to maxSpeed
            desiredVelocity = Vector3.Normalize(_targetPoint.Value - transform.position) * SteeringAgent.MaxCurrentSpeed;

            // Calculate steering velocity
            steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
            return steeringVelocity;
        }

    }
}