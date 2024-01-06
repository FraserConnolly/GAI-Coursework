using System.Collections.Generic;
using UnityEngine;
using GCU.FraserConnolly;
using GCU.FraserConnolly.AI.SteeringBehaviours;
using System;
using GCU.FraserConnolly.AI.Navigation;

public class AllyAgent : SteeringAgent
{
	//private WeaponManager _weaponManager;
	private Attack.AttackType attackType = Attack.AttackType.Gun;

	// Steering Behaviours
	private SeekToPoint _seekToPointBehaviour;
    private PathFollow  _pathFollowBehaviour;

    [SerializeField, Range(0f, 1f)]
    private float _inertia = 0.9f;


    protected override void InitialiseFromAwake()
	{
        // weapons

		//_weaponManager = gameObject.AddComponent<WeaponManager>();
     
		// steering behaviours - the order they are added to the gameObject determines their priority during Cooperative Arbitration
        gameObject.AddComponent<TreeAvoidance>();
        _pathFollowBehaviour = gameObject.AddComponent<PathFollow>();
        var sep = gameObject.AddComponent<Seperation>();
        var a = gameObject.AddComponent<Alignment>();

        a.enabled = false;
        ShowDebug = false;
        _pathFollowBehaviour.ShowDebugLines = true;
    }

    protected override void CooperativeArbitration()
	{
        // The base CooperativeArbitration method written by Hamid doesn't 
        // have any means of arbitrating between steering behaviours.
        // Make sure not to use it.
        //base.CooperativeArbitration();

        processDebugInput();

        SteeringVelocity = Vector3.zero;

        var steeringBehvaiours = new List<SteeringBehaviour>();

        GetComponents(steeringBehvaiours);
        foreach (SteeringBehaviour currentBehaviour in steeringBehvaiours)
        {
            // stop processing steering behaviours once the max steering speed has been reached.
            if (SteeringVelocity.sqrMagnitude < SteeringAgent.MaxSteeringSpeed * SteeringAgent.MaxSteeringSpeed)
            {
                if (currentBehaviour.enabled)
                {
                    Vector3 updateVelocity = currentBehaviour.UpdateBehaviour(this);

                    if (currentBehaviour is IWeightable weightable)
                    {
                        updateVelocity *= weightable.Weight;
                    }

                    SteeringVelocity += updateVelocity;
                }
            }
            else
            {

            }
        }

        // apply inertia to CurrentVelocity if there is no steering velocity
        if ( SteeringVelocity ==  Vector3.zero && CurrentVelocity != Vector3.zero)
        {
            CurrentVelocity *= _inertia;
        }

    }

    private void processDebugInput()
    {
        if (Input.GetMouseButton(0))
        {
            var startScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            var startWorldPosition = Camera.main.ScreenToWorldPoint(startScreenPosition);

            int x = (int)(startWorldPosition.x);
            int y = (int)(startWorldPosition.y);

            if (x < 0 || y < 0 || x >= Map.MapWidth || y >= Map.MapHeight)
            {
                return;
            }

            var navigable = GameData.Instance.Map.IsNavigatable(x, y);

            if (!navigable)
            {
                return;
            }

            Vector2Int startLocation = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            Vector2Int endLocation = new Vector2Int(x, y);
            var path = Pathfinding.GetPath(startLocation, endLocation, out _);
            _pathFollowBehaviour.setPath(path);
        }
    }

    protected override void UpdateDirection()
	{
		base.UpdateDirection();
	}
}
