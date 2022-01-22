using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public GameObject nextCheckpoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);
        if(nextCheckpoint) Gizmos.DrawLine(transform.position, nextCheckpoint.transform.position);
    }
}
