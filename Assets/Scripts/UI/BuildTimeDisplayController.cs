using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildTimeDisplayController : MonoBehaviour
{
    private GameStateController _stateController;
    
    private UnityEngine.UI.Text _timeTxt;

    private float _blinkTimer;
    private bool _blinkState;

    private float _blinkAlpha;
    private float _blinkOriginalAlpha;

    [Space] 
    public GameObject gameDirector;

    [Header("Elements")] 
    public GameObject time;
    public GameObject skipButton;
    public GameObject startButton;
    public GameObject icon;
    public GameObject gradient;

    [Header("Config")] 
    public float warningTime;
    public float blinkInterval;
    public float blinkEasing;

    void Start()
    {
        _stateController = gameDirector.GetComponent<GameStateController>();
        _timeTxt = time.GetComponent<UnityEngine.UI.Text>();
        _blinkOriginalAlpha = gradient.GetComponent<Image>().color.a;
    }
    
    void FixedUpdate()
    {
        if (_stateController.GameState == GameState.BuildingPhase)
        {
            if (_stateController.BuildingPhaseIsTimed)
            {
                time.SetActive(true);
                skipButton.SetActive(true);
                icon.SetActive(true);
                startButton.SetActive(false);
            
                _timeTxt.text = $"{_stateController.BuildingTimer:f2}";

                if (_stateController.BuildingTimer > warningTime)
                {
                    gradient.SetActive(false);
                    gradient.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    gradient.SetActive(true);
                    
                    if (_blinkTimer > 0)
                    {
                        _blinkTimer -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        _blinkState = !_blinkState;
                        _blinkTimer = blinkInterval;
                    }

                    _blinkAlpha += ((_blinkState ? _blinkOriginalAlpha : 0) - _blinkAlpha) * blinkEasing;
                    gradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
                }
            }
            else
            {
                time.SetActive(false);
                skipButton.SetActive(false);
                icon.SetActive(false);
                gradient.SetActive(false);
                startButton.SetActive(true);
            }
        }
        else
        {
            time.SetActive(false);
            skipButton.SetActive(false);
            icon.SetActive(false);
            gradient.SetActive(false);
            startButton.SetActive(false);
        }
        
    }
}
