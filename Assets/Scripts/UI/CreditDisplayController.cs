using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditDisplayController : MonoBehaviour
{
    private CreditController cc;
    private UnityEngine.UI.Text txt;
    
    // Start is called before the first frame update
    void Start()
    {
        cc = GameObject.Find("GameDirector").GetComponent<CreditController>();
        txt = GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = cc.CurrentCredits.ToString();
    }
}
