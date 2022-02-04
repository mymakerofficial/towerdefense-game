using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawnerController : MonoBehaviour
{
    private float _cooldown;
    private bool _active;
    private int _count;
    private GameObject _gameDirector;

    
    [Space]
    [FormerlySerializedAs("parrent")] public GameObject parent; // game object to instatiate enemy in
    
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
        _gameDirector = GameObject.Find("GameDirector");
    }
    
    private void FixedUpdate()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
            
        if (_active)
        {
            // timer for cooldown
            if (_cooldown > 0)
            {
                _cooldown -= Time.fixedDeltaTime;
            }
            else
            {
                // spawn enemy when timer is at 0
                SpawnEnemy();
                _cooldown = interval;
                _count++;
            }
        
            // stop when spawn amount has been reached
            if (_count >= amount && !infiniteAmount) _active = false;
        }
        else
        {
            _count = 0;

            // wait for start delay
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
        Instantiate(enemyObject, transform.position, transform.rotation, parent.transform);
    }
}
