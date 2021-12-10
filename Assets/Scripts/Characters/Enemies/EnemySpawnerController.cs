using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    private float _cooldown;

    public GameObject parrent; // game ogject to instatiate enemy in
    public GameObject enemyType; // game object to instantiate
    public bool active;
    
    public float Cooldown
    {
        get => _cooldown;
        private set => _cooldown = Mathf.Clamp(value, 0, 1);
    }
    
    void Start()
    {
        _cooldown = 1;
        active = true;
    }
    
    void FixedUpdate()
    {
        if (Cooldown > 0.9f) SpawnEnemy();
        
        Cooldown += 0.5f * Time.fixedDeltaTime;
    }

    public void SpawnEnemy()
    {
        if (!active) return;
        if (Cooldown == 0) return;
        
        Instantiate(enemyType, transform.position, transform.rotation, parrent.transform);

        Cooldown = 0;
    }
}
