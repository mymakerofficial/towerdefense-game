using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    public List<WaveSection> Sections;
}

[Serializable]
public class WaveSection
{
    public List<WaveEnemy> Enemies;
}

[Serializable]
public class WaveEnemy
{
    public GameObject Enemy;
    public int Amount;
    public float Interval;
    public float StartDelay;
}

public class WaveController : MonoBehaviour
{
    
    public List<Wave> Waves;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
