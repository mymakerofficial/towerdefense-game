using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum GameState {
    Idle,
    BuildingPhase,
    EnemyWavePhase,
    GameOver
}

public class GameStateController : MonoBehaviour
{
    private GameState _gameState;
    private bool _paused;
    private float _buildingTimer;
    private bool _firstWave;
    private bool _waitForWaveEnd;

    private InputMaster _controls;

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

    [Header("UI")] 
    public GameObject canvas;
    public GameObject gameOverCanvas;
    public GameObject pauseCanvas;

    [Header("Post Processing")] 
    public GameObject blurVolume;

    public GameState GameState => _gameState;
    public float BuildingTimer => _buildingTimer;
    public bool FirstWave => _firstWave;
    public bool BuildingPhaseIsTimed => !_firstWave;

    public bool Paused
    {
        get => _paused;
        set
        {
            if (value)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    
    public bool GameActive => (_gameState == GameState.BuildingPhase || _gameState == GameState.EnemyWavePhase) && !_paused;

    void Start()
    {
        _firstWave = true;
        StartBuilding();
    }
    
    void Awake()
    {
        _controls = new InputMaster();
        _controls.General.Pause.performed += _ => TogglePause();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();

    /// <summary>
    /// Reset entire game to start
    /// </summary>
    public void Restart()
    {
        Debug.Log("Reseting Game");
        
        // make list of all transforms that need to be deleted
        List<Transform> toBeDeleted = new List<Transform>();
        toBeDeleted.AddRange(enemies.GetComponentsInChildren<Transform>().ToList());
        toBeDeleted.AddRange(towers.GetComponentsInChildren<Transform>().ToList());
        toBeDeleted.AddRange(bullets.GetComponentsInChildren<Transform>().ToList());

        foreach (var tra in toBeDeleted)
        {
            if (tra.gameObject == enemies || tra.gameObject == towers || tra.gameObject == bullets) continue; // dont delete parrent
            
            Destroy(tra.gameObject);
        }
        
        // reset all Controllers
        waveController.GetComponent<WaveController>().Reset();
        stronghold.GetComponent<StrongholdController>().Reset();
        GetComponent<GameStatisticsController>().Reset();
        GetComponent<CreditController>().Start();
        Start();
    }

    public void StartBuilding()
    {
        _buildingTimer = buildingTime;
        
        _gameState = GameState.BuildingPhase;
        
        canvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        blurVolume.SetActive(false);
        
        Debug.Log("Starting building phase");
    }

    public void StartEnemyWave()
    {
        _gameState = GameState.EnemyWavePhase;
        _waitForWaveEnd = false;
        
        canvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        blurVolume.SetActive(false);
        
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
            
        gameObject.SendMessage("ReportWaveEnd");
        
        StartBuilding();
    }

    public void GameOver()
    {
        ResumeGame();
        
        _gameState = GameState.GameOver;
        _waitForWaveEnd = false;
        
        canvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        blurVolume.SetActive(true);
        
        Debug.Log("Game Over");
        
        gameObject.SendMessage("CalculateStatistics");
    }

    private void TogglePause()
    {
        Paused = !Paused;
    }

    public void PauseGame()
    {
        if(_gameState == GameState.GameOver) return;
        
        _paused = true;
        
        canvas.SetActive(false);
        blurVolume.SetActive(true);
        pauseCanvas.SetActive(true);

        Debug.Log("Paused Game!");
    }

    public void ResumeGame()
    {
        _paused = false;
        
        canvas.SetActive(true);
        blurVolume.SetActive(false);
        pauseCanvas.SetActive(false);
        
        Debug.Log("Resumed Game!");
    }

    private void FixedUpdate()
    {
        if (!_paused)
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

        // fade blur effect
        blurVolume.GetComponent<Volume>().weight += ((GameActive ? 0 : 1) - blurVolume.GetComponent<Volume>().weight) / 10;
    }
}
