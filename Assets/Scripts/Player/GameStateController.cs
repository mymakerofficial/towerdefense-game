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

    [Header("BuildingPhase")]
    public int buildingTime;

    [Header("WavePhase")]
    public GameObject waveController;

    [Header("Collections")]
    public GameObject enemies;
    public GameObject towers;
    public GameObject bullets;

    public GameState GameState => _gameState;
    public float BuildingTimer => _buildingTimer;
    
    void Start()
    {
        StartBuilding();
    }

    public void StartBuilding()
    {
        _gameState = GameState.BuildingPhase;

        _buildingTimer = buildingTime;
    }

    public void StartEnemyWave()
    {
        _gameState = GameState.EnemyWavePhase;
        
        waveController.SendMessage("StartNextWave");
    }

    private void FixedUpdate()
    {
        if (_gameState == GameState.BuildingPhase)
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
            
        }
    }
}
