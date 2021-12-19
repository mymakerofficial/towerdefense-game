using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

[Serializable]
public struct EnemyTowerBehaviour
{
    public string Name;
    public float MinDistance;
    public bool AttackWhenInRang;
    public bool AttackWhenDamaged;
    public bool MoveTowardsWhenInRange;
    public bool MoveTowardsWhenDamaged;
}

public class GenericEnemyController : MonoBehaviour
{
    public List<EnemyTowerBehaviour> behaviours = new List<EnemyTowerBehaviour>();

    private GameObject _activeTarget;
    private NavMeshAgent _agent;
    
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("UpdateTarget", 0, 1);
    }
    
    void UpdateTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        
        bool success = false;
        GameObject closest = null;
        EnemyTowerBehaviour closestBehaviour = new EnemyTowerBehaviour();
        float closestDistance = Single.MaxValue;
        
        // loop through all towers
        foreach (GameObject tower in towers)
        {
            EnemyTowerBehaviour behaviour = behaviours.Find(e => e.Name == tower.GetComponent<TowerDescriptor>().name);
            
            float distance = Vector3.Distance(transform.position, tower.transform.position);

            if (distance > closestDistance) continue; // skip if not closest
            if (distance > behaviour.MinDistance) continue; // skip if further then minumum distance

            success = true;
            closest = tower;
            closestBehaviour = behaviour;
            closestDistance = distance;
        }
        
        if (success && closestBehaviour.MoveTowardsWhenInRange)
        {
            _activeTarget = closest;
        }
        else
        {
            _activeTarget = GameObject.FindGameObjectsWithTag("Stronghold")[0];
        }
    }

    private void FixedUpdate()
    {
        _agent.destination = _activeTarget.transform.position;
    }
}
