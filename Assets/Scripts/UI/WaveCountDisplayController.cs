using UnityEngine;
using UnityEngine.UI;

public class WaveCountDisplayController : MonoBehaviour
{
    private GameStateController _gameState;
    private UnityEngine.UI.Text _txt;
    
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
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a;
    }
    
    void FixedUpdate()
    {
        int value = _gameState.WaveCount;
        
        _txt.text = value.ToString();

        if (value == 0) _lastValue = 0;

        if (value > _lastValue)
        {
            _blinkTemporary = true;
            _blinkState = true;
            _blinkTimer = blinkInterval;
            _lastValue = value;
        }

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

        _blinkAlpha += ((_blinkState ? _blinkOriginalAlpha : 0) - _blinkAlpha) * blinkEasing;
        warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
    }
}
