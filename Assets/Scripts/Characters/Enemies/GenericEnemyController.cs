using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[Serializable]
public enum FireCallOptions
{
    None,
    CallFire,
    SendTarget
}

public class GenericEnemyController : MonoBehaviour
{
    [Header("Attack Behaviour")] 
    [FormerlySerializedAs("AttackRange")] public float attackRange;
    [FormerlySerializedAs("AttackWhenInRang")] public bool attackWhenInRang;
    [FormerlySerializedAs("AttackWhenDamaged")] public bool attackWhenDamaged; //TODO Attack when damaged
    [Header("Move Behaviour")]
    [FormerlySerializedAs("MoveRange")] public float moveRange;
    [FormerlySerializedAs("MoveTowardsWhenInRange")] public bool moveTowardsWhenInRange;
    [FormerlySerializedAs("MoveTowardsWhenDamaged")] public bool moveTowardsWhenDamaged; //TODO Move towards when damaged
    public float stopDistance;
    [Header("Attack")]
    [FormerlySerializedAs("FireGameObject")] public GameObject fireGameObject;
    public FireCallOptions fireCallOptions;
    [Space]
    public Vector3 fireOffset;
    [Space]
    [FormerlySerializedAs("AttackCooldownSec")] public float attackCooldownSec;
    [FormerlySerializedAs("SelfDestructOnAttack")] public bool selfDestructOnAttack; // yes it does what you think it does
    [Header("Geometry")] 
    public GameObject head;
    public Vector3 headRotationOffset;
    public float headRotationDefault;

    private GameObject _activeMovementTarget;
    private GameObject _activeAttackTarget;
    private NavMeshAgent _agent;
    private float _cooldown;
    private GameObject _lastCheckpoint;
    
    private float _headRotation;

    private Vector3 _pausedVelocity;

    private static GameObject _stronghold;
    private GameObject _gameDirector;

    void Start()
    {
        _cooldown = 1;
        _stronghold = GameObject.FindGameObjectsWithTag("Stronghold")[0];
        _gameDirector = GameObject.Find("GameDirector");
        _agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }
    
    /// <summary>
    /// Finds target to attack / move towards
    /// </summary>
    void UpdateTarget()
    {
        GameObject closestMove = FindClosest(true);
        

        // set pathfinding target
        if (closestMove != null && Vector3.Distance(transform.position, closestMove.transform.position) < moveRange && moveTowardsWhenInRange)
        {
            _activeMovementTarget = closestMove;
            
            // calculate position infront of target with stop distance
            Vector3 dif = transform.position - _activeMovementTarget.transform.position;
            Vector3 targetPos = _activeMovementTarget.transform.position + dif.normalized * stopDistance;

            // set destination
            _agent.destination = targetPos;
        }
        else
        {
            // move towars next checkpoint by default
            _activeMovementTarget = NextCheckpoint();
            
            _agent.destination = _activeMovementTarget.transform.position;
        }

        if (fireGameObject != null)
        {
            GameObject closestAttack = FindClosest(false);
        
            // set target to attack
            if (closestAttack != null && Vector3.Distance(transform.position, closestAttack.transform.position) < attackRange  && attackWhenInRang)
            {
                _activeAttackTarget = closestAttack;
            }
        }
    }

    private GameObject NextCheckpoint()
    {
        List<GameObject> checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint").ToList();

        checkpoints.Add(_stronghold);

        
        GameObject top = null;
        float topValue = Single.MaxValue;
        foreach (var checkpoint in checkpoints)
        {
            float distanceToSelf = Vector3.Distance(transform.position, checkpoint.transform.position);
            float distanceToStronghold = Vector3.Distance(checkpoint.transform.position, _stronghold.transform.position);

            float value = distanceToSelf + distanceToStronghold / 1.5f; // 1.5 = random value that where i found out it works
            
            if (distanceToSelf < 3)
            {
                _lastCheckpoint = checkpoint;
                continue;
            }
            if (_lastCheckpoint == checkpoint) continue; // dont go back to last checkpoint
            if (value > topValue) continue;
            

            top = checkpoint;
            topValue = value;
        }

        return top;
    }

    private GameObject FindClosest(bool checkForCompletePath)
    {
        List<GameObject> targets = GameObject.FindGameObjectsWithTag("Tower").ToList();

        //targets.Add(_stronghold); // add stronghold so it gets attacked when closest

        bool success = false;
        GameObject closest = null;
        float closestDistance = Single.MaxValue;
        
        // loop through all towers
        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > closestDistance) continue;// skip if not closest

            if (checkForCompletePath)
            {
                if (distance > stopDistance) // no need to check if its gonna stop anyways
                {
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position, target.transform.position - transform.position,out hit))
                    {
                        if (hit.collider.gameObject != target) // check if hit is not target
                        {
                            continue;
                        }
                    }
                }
            }

            // save closest
            success = true;
            closest = target;
            closestDistance = distance;
        }

        if (!success)
        {
            return null;
        }

        return closest;
    }

    private void FixedUpdate()
    {
        if (!_gameDirector.GetComponent<GameStateController>().Paused)
        {
            if (_activeAttackTarget != null)
            {
                if (Vector3.Distance(transform.position, _activeAttackTarget.transform.position) > attackRange)
                    _activeAttackTarget = null;
           
                if (_cooldown > 0)
                {
                    _cooldown -= Time.fixedDeltaTime;
                }
                else
                {
                    Attack();
                }
            }
            
            // set head rotation
            if (head != null)
            {
                float rotation = headRotationDefault;
                if (_activeMovementTarget != null) rotation = 
                    GeneralMath.AngleTowardsPoint2D(head.transform.position, _activeMovementTarget.transform.position);
                if (_activeAttackTarget != null)
                    rotation = GeneralMath.AngleTowardsPoint2D(head.transform.position,
                        _activeAttackTarget.transform.position);
                _headRotation += (rotation - _headRotation) / 10;
                head.transform.rotation = Quaternion.Euler(headRotationOffset.x, headRotationOffset.y, _headRotation + headRotationOffset.z);
            }

            if(_agent.isStopped)
            {
                _agent.Resume();
                _pausedVelocity = _agent.velocity;
            }
        }
        else
        {
            if(!_agent.isStopped)
            {
                _pausedVelocity = _agent.velocity;
                _agent.Stop();
                _agent.velocity = Vector3.zero;
            }
        }
        
    }

    private void Attack()
    {
        if(_activeAttackTarget == null) return;
        
        _cooldown = attackCooldownSec;

        if (fireGameObject)
        {
            float angle = GeneralMath.AngleTowardsPoint2D(transform.position, _activeAttackTarget.transform.position);
            
            GameObject bullet = Instantiate(
                fireGameObject, 
                transform.position + fireOffset, 
                Quaternion.Euler(0, angle + 90, 0), 
                GameObject.Find("Bullets").transform
            );
            
            switch (fireCallOptions)
            {
                case FireCallOptions.CallFire:
                    bullet.SendMessage("Fire");
                    break;
                case FireCallOptions.SendTarget:
                    bullet.SendMessage("Fire", _activeAttackTarget);
                    break;
            }
        }

        if (selfDestructOnAttack)
        {
            CommitDie();
        }
    }

    /// <summary>
    /// you know what... i think you know what this does...
    /// </summary>
    private void CommitDie()
    {
        Destroy(gameObject);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(_activeMovementTarget != null) Gizmos.DrawSphere(_activeMovementTarget.transform.position, 0.2f);

        if (_activeAttackTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _activeAttackTarget.transform.position);
            if(_activeAttackTarget != null) Gizmos.DrawSphere(_activeAttackTarget.transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_agent.path.corners[_agent.path.corners.Length-1], 0.2f);
        for (int i = 0; i < _agent.path.corners.Length; i++)
        {
            Vector3 current = _agent.path.corners[i];
            Vector3 last = i == 0 ? transform.position : _agent.path.corners[i - 1];
            
            Gizmos.DrawLine(last, current);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, moveRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
