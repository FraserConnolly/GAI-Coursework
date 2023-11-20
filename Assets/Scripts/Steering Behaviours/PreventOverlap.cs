using System.Linq;
using UnityEngine;

/// <summary>
/// This implementation is based on Mat Buckland's implementation.
/// The implementation has been tested however the anti cheat system 
/// in use for this projects prevents direct changes to transform of 
/// an agent. As such this behaviour can not be used.
/// </summary>
public class PreventOverlap : SteeringBehaviour
{
    Map map;

    private void Awake()
    {
        map = GameData.Instance.Map;
    }

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
	{
        // This behaviour doesn't effect the velocity of the agent.
        // Instead it forces two agents to move away from each other by 
        // nudging their transform directly. This happens in late update to 
        // ensure that other steering behaviours have completed before the 
        // nudge occours.
        // Unfortunetly this 
        // This will only nudge Allied Units as Enemy units are outside of
        // my control in this assessment.
        return Vector3.zero;
	}

    private void LateUpdate()
    {
        ApplyNudging();
    }

    private void ApplyNudging()
    {

        Vector3 startingPositionOfSelf = gameObject.transform.position;

        // do to replace this list with one coming from a spacial paritioning system.
        var agents = SpacialPartitioning.GetAllAgents();

        foreach (var agent in agents) 
        { 
            if (agent == gameObject) 
            {
                // don't check ourselves for an overlap
                continue;
            }

            Vector3 toEntity = transform.position - agent.transform.position;
            float distanceFromEachOther = toEntity.magnitude;

            // In this game agent and target will have the same radious but
            // I have left them as two seperate fields to allow this code
            // to be more easily re-used should different agent sizes be
            // implmented in the future.
            float agentRadious = SteeringAgent.CollisionRadius;
            float targetRadious = SteeringAgent.CollisionRadius;

            float ammountOfOverlap = agentRadious + targetRadious - distanceFromEachOther;

            if ( ammountOfOverlap >= 0 )
            {
                // nudging is required

                var distanceToApply = (toEntity / toEntity.magnitude) * ammountOfOverlap;
                transform.position += distanceToApply;

                // From Hamid's SteeringAgent.cs
                // Check for collision with obstacles and prevent movement, but allow sliding collision if possible. Note: Sliding collision only
                // works in this context as obstacles are aligned to othogonals. Proper way to do this would be finding out normals of collision
                // and working out tangent to move along
                if (map.IsNavigatable((int)transform.position.x, (int)transform.position.y) == false)
                {
                    transform.position = startingPositionOfSelf;
                    transform.position += new Vector3(distanceToApply.x, 0.0f, 0.0f);
                    if (map.IsNavigatable((int)transform.position.x, (int)transform.position.y) == false)
                    {
                        transform.position = startingPositionOfSelf;
                        transform.position += new Vector3(0.0f, distanceToApply.y, 0.0f);
                        if (map.IsNavigatable((int)transform.position.x, (int)transform.position.y) == false)
                        {
                            transform.position = startingPositionOfSelf;
                        }
                    }
                }
            } // if ammountOfOverlap >= 0 
        }
    }
}
