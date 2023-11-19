using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{
    public class Seperation : SteeringBehaviour, IWeightable 
    {
        [SerializeField] 
        private float _threshold = 2f;
        
        [SerializeField] 
        private float _decayCoefficient = -25f;

        [SerializeField, Range(0f, 1f)]
        private float _weight = 1f;
        public float Weight => _weight;


        public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
        {
            var _targets = SpacialPartitioning.GetAllAgents();

            Vector3 steeringDirection = Vector3.zero;

            foreach (GameObject target in _targets)
            {
                if ( target == gameObject )
                {
                    continue;
                }

                Vector3 direction = target.transform.position - gameObject.transform.position;
                float distance = direction.magnitude;
                if (distance < _threshold)
                {
                    if ( distance == 0 )
                    {
                        // prevent div by 0 errors.
                        distance = 1;
                    }

                    float strength = Mathf.Min(_decayCoefficient / (distance * distance), 1f);
                    direction.Normalize();
                    steeringDirection += strength * direction;
                }
            }

            return steeringDirection;
        }

        private void OnDrawGizmosSelected()
        {
            if (ShowDebugLines)
            {
                Gizmos.DrawWireSphere(transform.position, _threshold);
            }
        }
    }
}