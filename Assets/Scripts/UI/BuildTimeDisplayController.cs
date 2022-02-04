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
        _timeTxt = time.GetComponent<Text>();
        _blinkOriginalAlpha = gradient.GetComponent<Image>().color.a; // save original alpha
    }
    
    void FixedUpdate()
    {
        if (_stateController.GameState == GameState.BuildingPhase)
        {
            if (_stateController.BuildingPhaseIsTimed)
            {
                // show time and skip button
                time.SetActive(true);
                skipButton.SetActive(true);
                icon.SetActive(true);
                
                // hide start button
                startButton.SetActive(false);
            
                // set time text
                _timeTxt.text = $"{_stateController.BuildingTimer:f2}";

                if (_stateController.BuildingTimer > warningTime)
                {
                    // hide gradient
                    gradient.SetActive(false);
                    gradient.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
                else
                {
                    // show gradient
                    gradient.SetActive(true);
                    
                    // blink state with timer
                    if (_blinkTimer > 0)
                    {
                        _blinkTimer -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        _blinkState = !_blinkState;
                        _blinkTimer = blinkInterval;
                    }

                    // ease alpha value
                    _blinkAlpha += ((_blinkState ? _blinkOriginalAlpha : 0) - _blinkAlpha) * blinkEasing;
                    gradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
                }
            }
            else
            {
                // hide time
                time.SetActive(false);
                skipButton.SetActive(false);
                icon.SetActive(false);
                gradient.SetActive(false);
                
                // show start button
                startButton.SetActive(true);
            }
        }
        else
        {
            // hide everything
            time.SetActive(false);
            skipButton.SetActive(false);
            icon.SetActive(false);
            gradient.SetActive(false);
            startButton.SetActive(false);
        }
        
    }
}
