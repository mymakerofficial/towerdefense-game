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
    public GameObject bulletType;

    private GameObject _activeMovementTarget;
    private GameObject _activeAttackTarget;
    private NavMeshAgent _agent;
    private float _cooldown;

    private static GameObject _stronghold;
    
    public float Cooldown
    {
        get => _cooldown;
        set => _cooldown = Mathf.Clamp(value, 0, 1);
    }
    
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

        
        // this is not final by any means
        if (_activeAttackTarget != null && Cooldown > 0.9f )
        {
            float angle = AngleTowardsPoint2D(transform.position, _activeAttackTarget.transform.position);
            GameObject bullet = Instantiate(
                Resources.Load<GameObject>("Prefabs/Bullets/GenericEnemyBullet"), 
                transform.position + new Vector3(0, 1.4f, 0), 
                Quaternion.Euler(0, angle + 90, 0), 
                GameObject.Find("Bullets").transform
            );
            bullet.SendMessage("Fire");
            Cooldown -= 0.05f;
        }
        
        Cooldown += 0.1f * Time.fixedDeltaTime;
        
        Debug.Log(Cooldown);
    }
    
    /// <summary>
    /// Get the angle between two points
    /// </summary>
    private float AngleTowardsPoint2D(Vector3 p1, Vector3 p2) // TODO This method is duplicated and needs to be moved into sepperate script
    {
        // calculate direction vector
        Vector2 dir2 = (new Vector2(p1.x, p1.z) -
                        new Vector2(p2.x, p2.z)).normalized;

        // calculate angle from direction vector
        float angle = (float)(Math.Atan2(dir2.y, -dir2.x) * (180 / Math.PI));

        return angle;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(_activeMovementTarget != null) Gizmos.DrawSphere(_activeMovementTarget.transform.position, 0.5f);
        
        Gizmos.color = Color.red;
        if(_activeAttackTarget != null) Gizmos.DrawSphere(_activeAttackTarget.transform.position, 0.5f);
    }
}
