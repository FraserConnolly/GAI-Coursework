using UnityEngine;

namespace GCU.FraserConnolly.AI.SteeringBehaviours
{
	public class SeekToPoint : SteeringBehaviour, IWeightable 
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

        private void Update()
        {
            if ( Input.GetMouseButton(0) )
			{
                var startScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
                var startWorldPosition = Camera.main.ScreenToWorldPoint(startScreenPosition);

				int x = (int)(startWorldPosition.x);
				int y = (int)(startWorldPosition.y);

				var navigable = GameData.Instance.Map.IsNavigatable(x, y);

				if (navigable)
				{
					SetTarget(new Vector3(x, y, 0f));
				}
			}
        }
    }
}