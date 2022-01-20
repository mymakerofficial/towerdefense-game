using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;


public class GenericEnemyController : MonoBehaviour
{
    [Header("Attack Behaviour")]
    public float AttackRange;
    public bool AttackWhenInRang;
    public bool AttackWhenDamaged; //TODO Attack when damaged
    [Header("Move Behaviour")]
    public float MoveRange;
    public bool MoveTowardsWhenInRange;
    public bool MoveTowardsWhenDamaged; //TODO Move towards when damaged
    [Header("Attack")]
    public GameObject FireGameObject;
    public float AttackCooldownSec;
    public bool SelfDestructOnAttack; // yes it does what you think it does

    private GameObject _activeMovementTarget;
    private GameObject _activeAttackTarget;
    private NavMeshAgent _agent;
    private float _cooldown;
    private GameObject _lastCheckpoint;

    private static GameObject _stronghold;

    void Start()
    {
        _cooldown = 1;
        _stronghold = GameObject.FindGameObjectsWithTag("Stronghold")[0];
        _agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }
    
    /// <summary>
    /// Finds target to attack / move towards
    /// </summary>
    void UpdateTarget()
    {
        GameObject closestMove = FindClosest(true);
        GameObject closestAttack = FindClosest(false);
        
        // set pathfinding target
        if (closestMove != null && Vector3.Distance(transform.position, closestMove.transform.position) < MoveRange && MoveTowardsWhenInRange)
        {
            _activeMovementTarget = closestMove;
        }
        else
        {
            // move towars next checkpoint by default
            _activeMovementTarget = NextCheckpoint();
        }
        
        // set target to attack
        if (closestAttack != null && Vector3.Distance(transform.position, closestAttack.transform.position) < AttackRange  && AttackWhenInRang)
        {
            _activeAttackTarget = closestAttack;
        }
        
        if(_activeMovementTarget) _agent.destination = _activeMovementTarget.transform.position;
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

        targets.Add(_stronghold); // add stronghold so it gets attacked when closest

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
                RaycastHit hit;
                if(Physics.Raycast(transform.position, target.transform.position - transform.position,out hit))
                {
                    if (hit.collider.gameObject != target) // check if hit is not target
                    {
                        continue;
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

        if (_cooldown > 0)
        {
            _cooldown -= Time.fixedDeltaTime;
        }
        else if(_activeAttackTarget != null)
        {
            Attack();
        }
    }

    private void Attack()
    {
        _cooldown = AttackCooldownSec;

        if (FireGameObject)
        {
            float angle = GeneralMath.AngleTowardsPoint2D(transform.position, _activeAttackTarget.transform.position);
            GameObject bullet = Instantiate(
                FireGameObject, 
                transform.position + new Vector3(0, 1.4f, 0), 
                Quaternion.Euler(0, angle + 90, 0), 
                GameObject.Find("Bullets").transform
            );
            bullet.SendMessage("Fire");
        }

        if (SelfDestructOnAttack)
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
        Gizmos.DrawWireSphere(transform.position, MoveRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
