using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GCU.FraserConnolly.PCG.Agents
{
    public abstract class Agent : MonoBehaviour
    {
        SteeringAgent _steeringAgent;
        protected bool isAlive => _steeringAgent != null ? _steeringAgent.Health > 0f : false;
        protected virtual void _Initialise ( SteeringAgent steeringAgent )
        {
            _steeringAgent = steeringAgent;
        }

        public abstract void Initialise ( SteeringAgent steeringAgent );

        protected virtual void Update()
        {
            if (isAlive)
            {
                var position = _steeringAgent.transform.position;

                var newPosition = TerrainRenderer.OffsetPoint;

                newPosition.x += position.x;
                newPosition.z += position.y;

                // to do set y to terrain height for this position;
                newPosition.y += TerrainRenderer.GetYOnTerrain(newPosition);

                transform.position = newPosition;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
