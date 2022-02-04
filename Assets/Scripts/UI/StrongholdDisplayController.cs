using UnityEngine;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("warninAmount")] public int warningAmount;
    public float blinkInterval;
    public float blinkEasing;
    
    void Start()
    {
        _strongholdController = GameObject.Find("Stronghold").GetComponent<StrongholdController>();
        _txt = text.GetComponent<Text>();
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a;// save original alpha
    }
    
    void FixedUpdate()
    {
        int value = _strongholdController.Health;
        
        // set text
        _txt.text = value.ToString();

        // if value is max reset last value
        if (value == _strongholdController.fullHealth) _lastValue = _strongholdController.fullHealth;

        // blink once if value decreased
        if (value < _lastValue && value < _strongholdController.fullHealth)
        {
            _blinkTemporary = true;
            _blinkState = true;
            _blinkTimer = blinkInterval;
            _lastValue = value;
        }

        // blink if value is below warning amount or value changed
        if (value < warningAmount || (_blinkTemporary && _blinkState))
        {
            // changed blink state with timer
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
