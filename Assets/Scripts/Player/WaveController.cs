using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("spawnerParrent")] public GameObject spawnerParent;
    [FormerlySerializedAs("enemyParrent")] public GameObject enemyParent;
    [Space]
    public Vector3 spawnerPosition;
    public Vector3 spawnerRotation;

    [Header("GameDirector")] 
    public GameObject gameDirector;

    public Wave CurrentWave => waves[_currentWaveIndex];

    public WaveSection CurrentSection => CurrentWave.sections[_currentSectionIndex];

    public int CurrentWaveIndex => _currentWaveIndex;
    public int CurrentSectionIndex => _currentSectionIndex;

    /// <summary>
    /// Reset all values and delete old spawners
    /// </summary>
    public void Reset()
    {
        _currentWaveIndex = 0;
        _currentSectionIndex = 0;
        _active = false;
        _sectionTimer = 0;
        _nextWaveIndex = 0;
        
        DeleteOldSpawners();
    }
    
    public void StartNextWave()
    {
        StartWave(_nextWaveIndex);
    }

    public void StartWave(int index)
    {
        if(_active) return; // dont start a wave while one is already running
        
        _currentWaveIndex = index; // set current wave
        _nextWaveIndex = index + 1; // set next wave
        if (_nextWaveIndex > waves.Count - 1) _nextWaveIndex = 0;

        _active = true; // set active

        Debug.Log($"Started wave {index}");
        
        StartSection(0); // start first section
    }

    private void StartSection(int index)
    {
        DeleteOldSpawners();

        if (index >= CurrentWave.sections.Count) // check if selected section is out of range and stop wave
        {
            StopWave();
            return;
        }
        
        _currentSectionIndex = index; // set current section

        Debug.Log($"Started wave {_currentWaveIndex}, section {_currentSectionIndex}");

        foreach (var enemy in CurrentSection.enemies) // create all spawners for this section
        {
            CreateSpawner(enemy);
        }

        _sectionTimer = CurrentSection.Duration; // set timer for duration of section
    }

    private void StopWave()
    {
        _active = false;
        
        Debug.Log($"Stopped wave {_currentWaveIndex}");
        
        gameDirector.SendMessage("WaitForWaveEnd");
    }

    void DeleteOldSpawners()
    {
        Transform[] oldSpawners = spawnerParent.GetComponentsInChildren<Transform>(); // get all spawners

        foreach (var spawner in oldSpawners)
        {
            if (spawner.gameObject == spawnerParent) continue; // dont delete parent
            
            Destroy(spawner.gameObject);
        }
    }
    
    private GameObject CreateSpawner(WaveEnemy enemy)
    {
        GameObject obj = new GameObject(); // create gameobject
        
        // setup gameobject
        obj.transform.SetParent(spawnerParent.transform);
        obj.transform.position = spawnerPosition;
        obj.transform.rotation = Quaternion.Euler(spawnerRotation);
        obj.name = $"EnemySpawner ({enemy.enemy.GetComponent<EnemyDescriptor>().name})";
        
        // add controller
        EnemySpawnerController controller = obj.AddComponent<EnemySpawnerController>();

        // set controller
        controller.enemyObject = enemy.enemy;
        controller.interval = enemy.interval;
        controller.startDelay = enemy.startDelay;
        controller.amount = enemy.amount;
        controller.parent = enemyParent;

        return obj;
    }

    private void FixedUpdate()
    {
        if (!_active || gameDirector.GetComponent<GameStateController>().Paused) return; // dont continue timer when game is paused

        // timer for sections
        if (_sectionTimer > 0)
        {
            _sectionTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // start next section when timer is at 0
            StartSection(_currentSectionIndex + 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawSphere(spawnerPosition, 0.3f);
    }
}
