using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditDisplayController : MonoBehaviour
{
    private CreditController cc;
    private UnityEngine.UI.Text txt;
    
    void Start()
    {
        cc = GameObject.Find("GameDirector").GetComponent<CreditController>();
        txt = GetComponent<UnityEngine.UI.Text>();
    }
    
    void Update()
    {
        txt.text = cc.CurrentCredits.ToString();
    }
}
