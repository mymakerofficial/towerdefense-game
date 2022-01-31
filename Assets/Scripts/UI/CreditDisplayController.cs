using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditDisplayController : MonoBehaviour
{
    private CreditController _creditController;
    private UnityEngine.UI.Text _txt;
    
    private float _blinkTimer;
    private bool _blinkState;

    private float _blinkAlpha;
    private float _blinkOriginalAlpha;

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
        _creditController = GameObject.Find("GameDirector").GetComponent<CreditController>();
        _txt = text.GetComponent<UnityEngine.UI.Text>();
        _blinkOriginalAlpha = warnGradient.GetComponent<Image>().color.a;
    }
    
    void FixedUpdate()
    {
        int value = (int)_creditController.CurrentCredits;
        
        _txt.text = value.ToString();
        
        if (value >= warninAmount)
        {
            warnGradient.SetActive(false);
            warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        else
        {
            warnGradient.SetActive(true);
                    
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
            warnGradient.GetComponent<Image>().color = new Color(1, 1, 1, _blinkAlpha);
        }
    }
}
