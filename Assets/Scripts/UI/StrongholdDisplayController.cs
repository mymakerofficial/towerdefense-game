using UnityEngine;
using UnityEngine.UI;

public class StrongholdDisplayController : MonoBehaviour
{
    private StrongholdController _strongholdController;
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
    public int warninAmount;
    public float blinkInterval;
    public float blinkEasing;
    
    void Start()
    {
        _strongholdController = GameObject.Find("Stronghold").GetComponent<StrongholdController>();
        _txt = text.GetComponent<Text>();
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a;
    }
    
    void FixedUpdate()
    {
        int value = _strongholdController.Health;
        
        _txt.text = value.ToString();

        if (value == _strongholdController.fullHealth) _lastValue = _strongholdController.fullHealth;

        if (value < _lastValue && value < _strongholdController.fullHealth)
        {
            _blinkTemporary = true;
            _blinkState = true;
            _blinkTimer = blinkInterval;
            _lastValue = value;
        }

        if (value < warninAmount || (_blinkTemporary && _blinkState))
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
