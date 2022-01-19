using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongholdController : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject EnemyParrent;
    public float AffectDistance;
    [Header("Health")]
    public int FullHealth;
    
    private int _health;

    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            if (_health + value >= 0)
            {
                _health = value;
            }
        }
    }

    public float HealthPercent => FullHealth / Health;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Health = FullHealth;
    }

    private void FixedUpdate()
    {
        foreach (Transform enemyTransform in EnemyParrent.transform) // loop through all enemies
        {
            if (Vector3.Distance(transform.position, enemyTransform.position) < AffectDistance) // check if enemy is in range
            {
                Destroy(enemyTransform.gameObject);
                Health--;
            }
        }

        if (Health == 0)
        {
            // TODO end game
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AffectDistance);
    }
}
