using System.Collections.Generic;
using UnityEngine;
using GCU.FraserConnolly;
using GCU.FraserConnolly.AI.SteeringBehaviours;

public class AllyAgent : SteeringAgent
{
	//private WeaponManager _weaponManager;
	private Attack.AttackType attackType = Attack.AttackType.Gun;

	// Steering Behaviours
	private SeekToPoint _seekToPointBehaviour;

    // to do - this is for debugging
    [SerializeField, Range(0f, 1f)]
    private float _inertia = 0.9f;

    protected override void InitialiseFromAwake()
	{
        // weapons

		//_weaponManager = gameObject.AddComponent<WeaponManager>();
     
		// steering behaviours
		var mouse = gameObject.AddComponent<SeekToMouse>();
		mouse.enabled = false;

        _seekToPointBehaviour = gameObject.AddComponent<SeekToPoint>();
        //gameObject.AddComponent<PreventOverlap>();
        gameObject.AddComponent<Seperation>();
        gameObject.AddComponent<Alignment>();
    }

	protected override void CooperativeArbitration()
	{
        // The base CooperativeArbitration method written by Hamid doesn't 
        // have any means of arbitrating between steering behaviours.
        // Make sure not to use it.
        //base.CooperativeArbitration();

        SteeringVelocity = Vector3.zero;

        var steeringBehvaiours = new List<SteeringBehaviour>();

        GetComponents(steeringBehvaiours);
        foreach (SteeringBehaviour currentBehaviour in steeringBehvaiours)
        {
            if (currentBehaviour.enabled)
            {
                Vector3 updateVelocity = currentBehaviour.UpdateBehaviour(this);

                if ( currentBehaviour is IWeightable weightable )
                {
                    updateVelocity *= weightable.Weight;
                }

                SteeringVelocity += updateVelocity;
            }
        }

        // apply inertia to CurrentVelocity if there is no steering velocity
        if ( SteeringVelocity ==  Vector3.zero && CurrentVelocity != Vector3.zero)
        {
            CurrentVelocity *= _inertia;
        }

    }

    protected override void UpdateDirection()
	{
		base.UpdateDirection();
	}
}
