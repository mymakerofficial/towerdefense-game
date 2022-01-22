using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTimeDisplayController : MonoBehaviour
{
    private GameStateController _stateController;
    
    private UnityEngine.UI.Text _timeTxt;

    private float _blinkTimer;
    private bool _blinkState;

    [Space] 
    public GameObject gameDirector;

    [Header("Elements")] 
    public GameObject time;
    public GameObject skipButton;
    public GameObject startButton;

    [Header("Config")] 
    public float warningTime;
    public float blinkInterval;

    [Header("Color")] 
    public Color textDefaultColor;
    public Color textWarningColor;

    void Start()
    {
        _stateController = gameDirector.GetComponent<GameStateController>();
        _timeTxt = time.GetComponent<UnityEngine.UI.Text>();
    }
    
    void FixedUpdate()
    {
        if (_stateController.GameState == GameState.BuildingPhase)
        {
            if (_stateController.BuildingPhaseIsTimed)
            {
                time.SetActive(true);
                skipButton.SetActive(true);
                startButton.SetActive(false);
            
                _timeTxt.text = $"{_stateController.BuildingTimer:f2}";

                if (_stateController.BuildingTimer > warningTime)
                {
                    _timeTxt.color = textDefaultColor;
                }
                else
                {
                    _timeTxt.color = _blinkState ? textWarningColor : textDefaultColor;
                
                    if (_blinkTimer > 0)
                    {
                        _blinkTimer -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        _blinkState = !_blinkState;
                        _blinkTimer = blinkInterval;
                    }
                }
            }
            else
            {
                time.SetActive(false);
                skipButton.SetActive(false);
                startButton.SetActive(true);
            }
        }
        else
        {
            time.SetActive(false);
            skipButton.SetActive(false);
            startButton.SetActive(false);
        }
        
    }
}
