using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Wave
{
    public List<WaveSection> Sections;
    
    public float Duration
    {
        get
        {
            float sum = 0;
            
            foreach (var section in Sections)
            {
                sum += section.Duration;
            }

            return sum;
        }
    }
}

[Serializable]
public class WaveSection
{
    public List<WaveEnemy> Enemies;

    public float Duration
    {
        get
        {
            float top = 0;
            
            foreach (var enemy in Enemies)
            {
                float duration = enemy.Duration + enemy.StartDelay;

                if (duration > top) top = duration;
            }

            return top;
        }
    }
}

[Serializable]
public class WaveEnemy
{
    [Header("Enemy")]
    public GameObject Enemy;
    public int Amount;
    [Header("Timing")]
    public float Interval;
    public float StartDelay;

    public float Duration
    {
        get
        {
            return Interval * Amount;
        }
    }
}

public class WaveController : MonoBehaviour
{
    private int _currentWaveIndex;
    private int _currentSectionIndex;
    private bool _active;
    private float _sectionTimer;
    private int _nextWaveIndex;
    
    [Header("Waves")]
    public List<Wave> waves;

    [Header("Spawners")] 
    public GameObject spawnerParrent;
    public GameObject enemyParrent;
    public Vector3 spawnerPosition;

    public Wave CurrentWave => waves[_currentWaveIndex];

    public WaveSection CurrentSection => CurrentWave.Sections[_currentSectionIndex];

    public int CurrentWaveIndex => _currentWaveIndex;
    public int CurrentSectionIndex => _currentSectionIndex;

    public void StartNextWave()
    {
        StartWave(_nextWaveIndex);
    }

    public void StartWave(int index)
    {
        if(_active) return; // dont start a wave while one is already running
        
        _currentWaveIndex = index; // set current wave
        _nextWaveIndex = index + 1 % waves.Count; // set next wave

        _active = true; // set active

        Debug.Log($"Started wave {index}");
        
        StartSection(0); // start first section
    }

    private void StartSection(int index)
    {
        DeleteOldSpawners();

        if (index >= CurrentWave.Sections.Count) // check if selected section is out of range and stop wave
        {
            StopWave();
            return;
        }
        
        _currentSectionIndex = index; // set current section

        Debug.Log($"Started section {index}");
            
        foreach (var enemy in CurrentSection.Enemies) // create all spawners for this section
        {
            CreateSpawner(enemy);
        }

        _sectionTimer = CurrentSection.Duration; // set timer for duration of section
    }

    private void StopWave()
    {
        _active = false;
        
        Debug.Log($"Stopped wave {_currentWaveIndex}");
    }

    void DeleteOldSpawners()
    {
        Transform[] oldSpawners = spawnerParrent.GetComponentsInChildren<Transform>(); // get all spawners

        foreach (var spawner in oldSpawners)
        {
            if (spawner.gameObject == spawnerParrent) continue; // dont delete parrent
            
            Destroy(spawner.gameObject);
        }
    }
    
    private GameObject CreateSpawner(WaveEnemy enemy)
    {
        GameObject obj = new GameObject(); // create gameobject
        
        // setup gameobject
        obj.transform.SetParent(spawnerParrent.transform);
        obj.transform.position = spawnerPosition;
        obj.name = $"EnemySpawner ({enemy.Enemy.GetComponent<EnemyDescriptor>().name})";
        
        // add controller
        EnemySpawnerController controller = obj.AddComponent<EnemySpawnerController>();

        // set controller
        controller.EnemyObject = enemy.Enemy;
        controller.Interval = enemy.Interval;
        controller.StartDelay = enemy.StartDelay;
        controller.Amount = enemy.Amount;
        controller.Parrent = enemyParrent;

        return obj;
    }

    private void FixedUpdate()
    {
        if (_active)
        {
            // timer for sections
            if (_sectionTimer > 0)
            {
                _sectionTimer -= Time.fixedDeltaTime;
            }
            else
            {
                StartSection(_currentSectionIndex + 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawSphere(spawnerPosition, 0.3f);
    }
}
