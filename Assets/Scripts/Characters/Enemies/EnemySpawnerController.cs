using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    private float _cooldown;
    private bool _active;
    private int _count;

    [Space]
    [Space]
    public GameObject parrent; // game ogject to instatiate enemy in
    
    [Header("Spawn")]
    public GameObject enemyObject; // game object to instantiate
    [Space]
    public float interval;
    public float startDelay;
    [Space]
    public bool autostart = true;

    [Header("Amount")] 
    public int amount;
    public bool infiniteAmount;

    void Start()
    {
        _cooldown = startDelay;
        _active = false;
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
                _cooldown = interval;
                _count++;
            }
            
            if (_count >= amount && !infiniteAmount) _active = false;
        }
        else
        {
            _count = 0;

            if (_cooldown >= 0)
            {
                _cooldown -= Time.fixedDeltaTime;
            }
            else if (autostart)
            {
                _active = true;
                _cooldown = 0;
            }
        }
    }

    public void SpawnEnemy()
    {
        Instantiate(enemyObject, transform.position, transform.rotation, parrent.transform);
    }
}
