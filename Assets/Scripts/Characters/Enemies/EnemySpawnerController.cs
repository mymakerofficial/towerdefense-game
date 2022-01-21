using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    private float _cooldown;
    private bool _active;
    private bool _firstStart = true;
    private int _count;
    private bool _autostart = true;
    
    [Space]
    [Space]
    public GameObject Parrent; // game ogject to instatiate enemy in
    
    [Header("Spawn")]
    public GameObject EnemyObject; // game object to instantiate
    [Space]
    public float Interval;
    public float StartDelay;

    [Header("Amount")] 
    public int Amount;
    public bool InfiniteAmount;

    void Start()
    {
        
    }
    
    private void FixedUpdate()
    {
        if (_active)
        {
            if (_cooldown > 0)
            {
                _cooldown -= Time.fixedDeltaTime;
            }
            else
            {
                SpawnEnemy();
                _cooldown = Interval;
                _count++;
            }
        }
        else
        {
            _count = 0;
            
            if (_firstStart)
            {
                _cooldown = StartDelay;
                _firstStart = false;
            }
            
            if (_cooldown >= 0)
            {
                _cooldown -= Time.fixedDeltaTime;
            }
            else if (_autostart)
            {
                _autostart = false;
                _active = true;
                _cooldown = 0;
            }
        }

        if (_count >= Amount) _active = false;
    }

    public void SpawnEnemy()
    {
        Instantiate(EnemyObject, transform.position, transform.rotation, Parrent.transform);
    }
}
