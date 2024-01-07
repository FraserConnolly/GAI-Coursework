using System.Collections.Generic;
using UnityEngine;
using GCU.FraserConnolly;
using GCU.FraserConnolly.AI.SteeringBehaviours;
using System;
using GCU.FraserConnolly.AI.Navigation;
using System.Diagnostics;
using GCU.FraserConnolly.AI;

public class AllyAgent : SteeringAgent
{
	//private WeaponManager _weaponManager;
	private Attack.AttackType attackType = Attack.AttackType.Gun;

	// Steering Behaviours
	private SeekToPoint _seekToPointBehaviour;
    private PathFollow  _pathFollowBehaviour;
    private Evade       _evade;

    [SerializeField, Range(0f, 1f)]
    private float _inertia = 0.9f;


    enum AllyState
    {
        Idle,
        AttackTarget,
        MoveToTarget,
        BetweenAttacks
    }

    private AllyState _state = AllyState.Idle;
    private GameObject _enemyTarget = null;
    private float _enemyTargetDistance;
    private Attack.AttackType selectedWeapon = Attack.AttackType.None;
    private float previousHealth = 1f;
    private WeaponSelection _weaponSelection;

    protected override void InitialiseFromAwake()
	{
        // weapons
        _weaponSelection = FindAnyObjectByType<WeaponSelection>(); 
     
		// steering behaviours - the order they are added to the gameObject determines their priority during Cooperative Arbitration
        gameObject.AddComponent<TreeAvoidance>();
        _evade = gameObject.AddComponent<Evade>();
        _evade.enabled = false;
        _pathFollowBehaviour = gameObject.AddComponent<PathFollow>();
        var sep = gameObject.AddComponent<Seperation>();
     
        ShowDebug = false;
    }

    protected override void CooperativeArbitration()
	{
        // The base CooperativeArbitration method written by Hamid doesn't 
        // have any means of arbitrating between steering behaviours.
        // Make sure not to use it.
        //base.CooperativeArbitration();

        processDebugInput();

        if ( _enemyTarget != null )
        {
            _enemyTargetDistance = (_enemyTarget.transform.position - transform.position).magnitude;
        }
        else
        {
            _enemyTargetDistance = 0f;
        }

        switch (_state)
        {
            default:
            case AllyState.Idle:
                ProcessIdle();
                break;
            case AllyState.AttackTarget:
                ProcessAttack();
                return; // deliberately don't apply the steering behaviours during an attack
            case AllyState.MoveToTarget:
                ProcessMoveToTarget();
                break;
            case AllyState.BetweenAttacks:
                ProcessBetweenAttacks();
                break;
        }

        ProcessSteeringBehaviours();
        previousHealth = Health;
    }

    private void ProcessSteeringBehaviours ( )
    {
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
        if (SteeringVelocity == Vector3.zero && CurrentVelocity != Vector3.zero)
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

        if ( Input.GetMouseButton(1))
        {
            if ( CanAttack(Attack.AttackType.Gun) )
            {
                AttackWith(Attack.AttackType.Gun);
            }
        }
    }

    private void ProcessIdle()
    {
        if ( GameData.Instance.enemies.Count > 0 )
        {
            // select a target
            selectTarget();
        }

        if ( _enemyTarget != null )
        {
            _state = AllyState.MoveToTarget;
        }
    }

    private void ProcessAttack()
    {
        if ( _enemyTarget == null )
        {
            _state = AllyState.Idle;
            _evade.enabled = false;
            return;
        }

        if (selectedWeapon == Attack.AttackType.None)
        {
            _state = AllyState.BetweenAttacks;
            return;
        }

        AttackWith(selectedWeapon);

        _state = AllyState.BetweenAttacks;
    }

    private void ProcessMoveToTarget()
    {
        if ( _enemyTarget == null )
        {
            _state = AllyState.Idle;
            return;
        }

        if ( _enemyTargetDistance < 15f && Node.HasLineOfSightBetweenPoints( transform.position, _enemyTarget.transform.position ))
        {
            _state = AllyState.BetweenAttacks;
            return;
        }

        if ( previousHealth != Health )
        {
            // have been attacked whilst travelling
            // return to the idle state so that a new target will be selected, this will presumably be the nearest.
            _state = AllyState.Idle;
            return;
        }
        
        if ( ! _pathFollowBehaviour.isFollowingPath )
        {
            var path = Pathfinding.GetPath(transform.position, _enemyTarget.transform.position);
            _pathFollowBehaviour.setPath(path);
        }
    }

    private void ProcessBetweenAttacks()
    {
        if ( _enemyTarget == null )
        {
            _state = AllyState.Idle;
            _evade.enabled = false;
            return;
        }

        if ( _enemyTargetDistance > 15f )
        {
            _state = AllyState.MoveToTarget;
            _evade.enabled = false;
            return;
        }

        if (!_evade.enabled)
        {
            var pointBetweenAgentAndTarget = (_enemyTarget.transform.position + transform.position) / 2;
            _evade.SetTarget(pointBetweenAgentAndTarget);
        }

        // select weapon
        selectedWeapon = _weaponSelection?.GetWeapon(_enemyTargetDistance, (float)GameData.Instance.AllyRocketsAvailable) ?? Attack.AttackType.None;

        if ( selectedWeapon != Attack.AttackType.None )
        {
            _pathFollowBehaviour.clearPath();

            if ( CanAttack(selectedWeapon) )
            {
                _state = AllyState.AttackTarget;
            }
        }
    }

    private void selectTarget()
    {
        // to do - implement some Fuzzy AI actual logic here.

        GameObject target = null;
        float shortestDistance = float.MaxValue;

        foreach (var enemyUnit in GameData.Instance.enemies)
        {
            if ( enemyUnit == null  )
            {
                continue;
            }

            float distance = (enemyUnit.transform.position - transform.position).sqrMagnitude;

            if ( distance < shortestDistance ) 
            {
                shortestDistance = distance;
                target = enemyUnit;
            }
        }

        _enemyTarget = target;
        _pathFollowBehaviour.clearPath();
    }

    protected override void UpdateDirection()
	{
        if ( _state == AllyState.AttackTarget || _state == AllyState.BetweenAttacks )
        {
            // look at target being attacked.
            transform.up = Vector3.Normalize( _enemyTarget.transform.position - transform.position );
            return;
        }

		base.UpdateDirection();
	}
}
