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

    private static GameObject _stronghold;

    void Start()
    {
        _cooldown = 1;
        _stronghold = GameObject.FindGameObjectsWithTag("Stronghold")[0];
        _agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("UpdateTarget", 0, 1);
    }
    
    /// <summary>
    /// Finds target to attack / move towards
    /// </summary>
    void UpdateTarget()
    {
        List<GameObject> targets = GameObject.FindGameObjectsWithTag("Tower").ToList();

        targets.Add(_stronghold); // add stronghold so it gets attacked when closest
        
        bool success = false;
        GameObject closest = null;
        float closestDistance = Single.MaxValue;
        
        // loop through all towers
        foreach (GameObject t in targets)
        {
            float distance = Vector3.Distance(transform.position, t.transform.position);

            if (distance > closestDistance) continue; // skip if not closest

            // save closest
            success = true;
            closest = t;
            closestDistance = distance;
        }
        
        // set pathfinding target
        if (success && closestDistance < MoveRange && MoveTowardsWhenInRange)
        {
            _activeMovementTarget = closest;
        }
        else
        {
            // move towars stronghold by default
            _activeMovementTarget = _stronghold;
        }
        
        // set target to attack
        if (success && closestDistance < AttackRange  && AttackWhenInRang)
        {
            _activeAttackTarget = closest;
        }
    }

    private void FixedUpdate()
    {
        _agent.destination = _activeMovementTarget.transform.position;
        
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
        
        float angle = GeneralMath.AngleTowardsPoint2D(transform.position, _activeAttackTarget.transform.position);
        GameObject bullet = Instantiate(
            FireGameObject, 
            transform.position + new Vector3(0, 1.4f, 0), 
            Quaternion.Euler(0, angle + 90, 0), 
            GameObject.Find("Bullets").transform
        );
        bullet.SendMessage("Fire");
        
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
