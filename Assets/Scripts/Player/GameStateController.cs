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
    private bool _mainMenuActive;

    private int _waveCount;
    
    private bool _strongholdWarningShown;
    private int _strongholdWaveStartHealth;
    
    private InputMaster _controls;

    [Header("MainMenu")]
    public GameObject mainMenuEnemyDestroyer;
    public GameObject mainMenuDesktopButtons;
    public GameObject mainMenuWebButtons;

    [Header("Towers")]
    public GameObject towerPlacementController;
    public GameObject towerModifyController;
    
    [Header("BuildingPhase")]
    public int buildingTime;

    [Header("WavePhase")]
    public GameObject waveController;

    [Header("Collections")]
    public GameObject enemies;
    public GameObject towers;
    public GameObject bullets;
    [Space] 
    public GameObject protectedZonesCircles;

    [Header("Stronghold")] 
    public GameObject stronghold;

    [Header("UI")] 
    public GameObject canvas;
    public GameObject gameOverCanvas;
    public GameObject pauseCanvas;
    public GameObject mainMenuCanvas;
    public GameObject notificationPanel;

    [Header("Cameras")] 
    public GameObject mainCamera;
    public GameObject menuCamera;

    [Header("Post Processing")] 
    public GameObject blurVolume;

    [Header("Notifications")] 
    public bool showNotifications;
    [Space]
    public string waveStartText;
    public string waveEndText;
    public string strongholdAttackText;

    public GameState GameState => _gameState;
    public float BuildingTimer => _buildingTimer;
    public bool FirstWave => _firstWave;
    public bool BuildingPhaseIsTimed => !_firstWave;
    public int WaveCount => _waveCount;

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
        MainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        _mainMenuActive = true;
        _paused = false;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            mainMenuDesktopButtons.SetActive(false);
            mainMenuWebButtons.SetActive(true);
        }
        else
        {
            mainMenuDesktopButtons.SetActive(true);
            mainMenuWebButtons.SetActive(false);
        }
        
        towerPlacementController.GetComponent<TowerPlacementController>().CancelPlacement();
        
        mainMenuEnemyDestroyer.SetActive(true);
        
        EnableMainMenuCanvas();
        
        mainCamera.GetComponent<Camera>().enabled = false;
        menuCamera.GetComponent<Camera>().enabled = true;
        
        Debug.Log(Application.platform.ToString());
        
        Reset();
        StartEnemyWave();
    }
    
    public void StartGame()
    {
        Reset();
        
        _mainMenuActive = false;
        _paused = false;
        mainMenuEnemyDestroyer.SetActive(false);
        mainCamera.GetComponent<Camera>().enabled = true;
        menuCamera.GetComponent<Camera>().enabled = false;

        _firstWave = true;
        _waveCount = 0;
        
        EnableInGameCanvas();
        
        StartBuilding();
    }
    
    void Awake()
    {
        _controls = new InputMaster();
        _controls.General.Pause.performed += _ => TogglePause();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();

    private void Reset()
    {
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
        towerModifyController.GetComponent<TowerModifyController>().UnSelect();
    }

    private void EnableInGameCanvas()
    {
        canvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        blurVolume.SetActive(false);
        
        protectedZonesCircles.SetActive(true);
    }
    
    private void EnablePauseCanvas()
    {
        canvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        blurVolume.SetActive(true);
        
        //protectedZonesCircles.SetActive(false);
    }
    
    private void EnableGameOverCanvas()
    {
        canvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        blurVolume.SetActive(true);
        
        protectedZonesCircles.SetActive(false);
    }
    
    private void EnableMainMenuCanvas()
    {
        canvas.SetActive(false);
        gameOverCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        blurVolume.SetActive(true);
        
        protectedZonesCircles.SetActive(false);
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
        _strongholdWaveStartHealth = stronghold.GetComponent<StrongholdController>().Health;
        _strongholdWarningShown = false;
        _waveCount++;

        Debug.Log("Starting enemy wave phase");
        if(showNotifications) notificationPanel.SendMessage("ShowNotification", waveStartText);

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
        if(showNotifications) notificationPanel.SendMessage("ShowNotification", waveEndText);
            
        gameObject.SendMessage("ReportWaveEnd");
        
        StartBuilding();
    }

    public void GameOver()
    {
        ResumeGame();
        
        _gameState = GameState.GameOver;
        _waitForWaveEnd = false;

        EnableGameOverCanvas();
        
        notificationPanel.SendMessage("Hide");
        
        Debug.Log("Game Over");
        
        gameObject.SendMessage("CalculateStatistics");
    }

    private void TogglePause()
    {
        Paused = !Paused;
    }

    public void PauseGame()
    {
        if(_gameState == GameState.GameOver || _mainMenuActive) return;
        
        _paused = true;
        
        
        
        EnablePauseCanvas();

        Debug.Log("Paused Game!");
    }

    public void ResumeGame()
    {
        _paused = false;
        
        EnableInGameCanvas();
        
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
                if (!_mainMenuActive)
                {
                    if (stronghold.GetComponent<StrongholdController>().Health < _strongholdWaveStartHealth &&
                        !_strongholdWarningShown)
                    {
                        if(showNotifications) notificationPanel.SendMessage("ShowNotification", strongholdAttackText);
                        _strongholdWarningShown = true;
                    }
                    
                    if(_waitForWaveEnd && enemies.GetComponentsInChildren<Transform>().Length == 1)
                    {
                        EndWave();
                    }

                    if (stronghold.GetComponent<StrongholdController>().HealthPercent == 0)
                    {
                        GameOver();
                    }
                }
                else
                {
                    if(_waitForWaveEnd && enemies.GetComponentsInChildren<Transform>().Length == 1)
                    {
                        _firstWave = false;
                        _waitForWaveEnd = false;
        
                        Debug.Log("Ended enemy wave phase");
            
                        gameObject.SendMessage("ReportWaveEnd");
                        
                        StartEnemyWave();
                    }
                }
            }
        }

        // fade blur effect
        blurVolume.GetComponent<Volume>().weight += ((GameActive && !_mainMenuActive ? 0 : 1) - blurVolume.GetComponent<Volume>().weight) / 10;
    }
}
