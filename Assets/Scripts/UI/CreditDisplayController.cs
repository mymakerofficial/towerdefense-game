using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreditDisplayController : MonoBehaviour
{
    private CreditController _creditController;
    private Text _txt;
    
    private float _blinkTimer;
    private bool _blinkState;

    private float _blinkAlpha;
    private float _blinkOriginalAlpha;

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
        _creditController = GameObject.Find("GameDirector").GetComponent<CreditController>();
        _txt = text.GetComponent<Text>();
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a; // save original alpha
    }
    
    void FixedUpdate()
    {
        int value = (int)_creditController.CurrentCredits;
        
        // set text
        _txt.text = value.ToString();
        
        if (value >= warningAmount)
        {
            // hide gradient
            warnGradient.SetActive(false);
            warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        else
        {
            // show gradient
            warnGradient.SetActive(true);
                    
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
            warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
        }
    }
}
