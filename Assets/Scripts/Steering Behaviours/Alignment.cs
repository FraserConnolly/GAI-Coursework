using System.Linq;
using TMPro;
using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{

    public class Alignment : SteeringBehaviour, IWeightable
    {
        [SerializeField]
        private float _alignDistance = 4f;

        [SerializeField, Range(0f, 1f)]
        private float _weight = 1f;
        public float Weight => _weight;

        public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
        {
            // to do replace with a SpacialPartitioning version of this get.
            var targets = SpacialPartitioning.GetAllAgents();

            if ( ! targets.Any() )
            {
                return Vector3.zero;
            }

            Vector3 direction = Vector3.zero;
            int count = 0;

            foreach (GameObject target in targets)
            {
                if (target == gameObject)
                {
                    // don't check our own alignment
                    continue;
                }

                SteeringAgent agent = target?.GetComponent<SteeringAgent>();
                
                if (agent == null)
                {
                    continue;
                }

                Vector3 targetDir = target.transform.position - gameObject.transform.position;
                
                if (targetDir.magnitude < _alignDistance)
                {
                    direction += agent.CurrentVelocity; // agent.transform.up;
                    count++;
                }
            }
            
            if (count > 0)
            {
                direction = direction / count;
            }

            if ( direction.normalized == transform.up )
            {
                direction = Vector3.zero;
            }

            desiredVelocity = direction;
            steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;

            return steeringVelocity;
        }

        private void OnDrawGizmosSelected()
        {
            //if (ShowDebugLines)
            //{
            //    Gizmos.DrawWireSphere(transform.position, _alignDistance);
            //}
        }
    }
}
