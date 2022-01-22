using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Idle,
    BuildingPhase,
    EnemyWavePhase,
    GameOver
}

public class GameStateController : MonoBehaviour
{
    private GameState _gameState;
    private float _buildingTimer;
    private bool _firstWave;
    private bool _waitForWaveEnd;

    [Header("BuildingPhase")]
    public int buildingTime;

    [Header("WavePhase")]
    public GameObject waveController;

    [Header("Collections")]
    public GameObject enemies;
    public GameObject towers;
    public GameObject bullets;

    [Header("Stronghold")] 
    public GameObject stronghold;

    public GameState GameState => _gameState;
    public float BuildingTimer => _buildingTimer;
    public bool FirstWave => _firstWave;
    public bool BuildingPhaseIsTimed => !_firstWave;
    
    void Start()
    {
        _firstWave = true;
        StartBuilding();
    }

    public void StartBuilding()
    {
        _buildingTimer = buildingTime;
        
        _gameState = GameState.BuildingPhase;
        
        Debug.Log("Starting building phase");
    }

    public void StartEnemyWave()
    {
        _gameState = GameState.EnemyWavePhase;
        _waitForWaveEnd = false;
        
        Debug.Log("Starting enemy wave phase");

        waveController.SendMessage("StartNextWave");
    }

    public void WaitForWaveEnd()
    {
        _waitForWaveEnd = true;
        
        Debug.Log("Waiting for all enemies to despawn");
    }

    public void EndWave()
    {
        _firstWave = false;
        _waitForWaveEnd = false;
        
        Debug.Log("Ended enemy wave phase");
        
        StartBuilding();
    }

    private void GameOver()
    {
        _gameState = GameState.GameOver;
        _waitForWaveEnd = false;
        
        Debug.Log("Game Over");
    }

    private void FixedUpdate()
    {
        if (_gameState == GameState.BuildingPhase && BuildingPhaseIsTimed)
        {
            if (_buildingTimer > 0)
            {
                _buildingTimer -= Time.fixedDeltaTime;
            }
            else
            {
                StartEnemyWave();
            }
        }

        if (_gameState == GameState.EnemyWavePhase)
        {
            if(_waitForWaveEnd && enemies.GetComponentsInChildren<Transform>().Length == 1)
            {
                EndWave();
            }

            if (stronghold.GetComponent<StrongholdController>().HealthPercent == 0)
            {
                GameOver();
            }
        }
    }
}
