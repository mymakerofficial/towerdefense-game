using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeContoller : MonoBehaviour
{
    public float Interval = 0.5f;
    public float DamageAmount = 1f;
    
    private List<GameObject> collisions = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DealDamage", 0f, Interval);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.GetComponent<HealthController>()) collisions.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        collisions.Remove(col.gameObject);
    }

    private void DealDamage()
    {
        for(int i = 0 ; i < collisions.Count ; i++)
        {
            GameObject o = collisions[i];
            
            o.SendMessage("ApplyDamage",DamageAmount);
        };
    }
}
