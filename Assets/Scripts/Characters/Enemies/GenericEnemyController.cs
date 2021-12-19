using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

[Serializable]
public struct EnemyBehaviour
{
    public float MinAttackDistance;
    public bool AttackWhenInRang;
    public bool AttackWhenDamaged;
    public float MinMoveDistance;
    public bool MoveTowardsWhenInRange;
    public bool MoveTowardsWhenDamaged;
}

public class GenericEnemyController : MonoBehaviour
{
    public EnemyBehaviour behaviour;

    private GameObject _activeMovementTarget;
    private GameObject _activeAttackTarget;
    private NavMeshAgent _agent;

    private static GameObject _stronghold;
    
    void Start()
    {
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
        if (success && closestDistance < behaviour.MinMoveDistance && behaviour.MoveTowardsWhenInRange)
        {
            _activeMovementTarget = closest;
        }
        else
        {
            // move towars stronghold by default
            _activeMovementTarget = _stronghold;
        }
        
        // set target to attack
        if (success && closestDistance < behaviour.MinAttackDistance  && behaviour.AttackWhenInRang)
        {
            _activeAttackTarget = closest;
        }
    }

    private void FixedUpdate()
    {
        _agent.destination = _activeMovementTarget.transform.position;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_activeMovementTarget.transform.position, 0.5f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_activeAttackTarget.transform.position, 0.5f);
    }
}
