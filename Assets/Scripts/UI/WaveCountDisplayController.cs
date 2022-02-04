using UnityEngine;
using UnityEngine.UI;

public class WaveCountDisplayController : MonoBehaviour
{
    private GameStateController _gameState;
    private Text _txt;
    
    private bool _blinkTemporary;
    
    private float _blinkTimer;
    private bool _blinkState;

    private float _blinkAlpha;
    private float _blinkOriginalAlpha;

    private int _lastValue;

    [Space]
    public GameObject text;
    public GameObject icon;
    public GameObject warnGradient;
    
    [Header("Config")]
    public float blinkInterval;
    public float blinkEasing;
    
    void Start()
    {
        _gameState = GameObject.Find("GameDirector").GetComponent<GameStateController>();
        _txt = text.GetComponent<Text>();
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a; // save original alpha
    }
    
    void FixedUpdate()
    {
        int value = _gameState.WaveCount;
        
        _txt.text = value.ToString();

        // reset last value when value is 0
        if (value == 0) _lastValue = 0;

        // blink if value increased
        if (value > _lastValue)
        {
            _blinkTemporary = true;
            _blinkState = true;
            _blinkTimer = blinkInterval;
            _lastValue = value;
        }

        // stay on for time
        if (_blinkTemporary && _blinkState)
        {
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

        // ease alpha value
        _blinkAlpha += ((_blinkState ? _blinkOriginalAlpha : 0) - _blinkAlpha) * blinkEasing;
        warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
    }
}
