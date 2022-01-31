using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongholdDisplayController : MonoBehaviour
{
    private StrongholdController sh;
    private UnityEngine.UI.Text txt;
    
    void Start()
    {
        sh = GameObject.Find("Stronghold").GetComponent<StrongholdController>();
        txt = GetComponent<UnityEngine.UI.Text>();
    }
    
    void Update()
    {
        txt.text = sh.Health.ToString();
    }
}
