using System;
using UnityEngine;

[Serializable]
public class WaveEnemy
{
    [Header("Enemy")]
    public GameObject enemy;
    public int amount;
    [Header("Timing")]
    public float interval;
    public float startDelay;

    public float Duration
    {
        get
        {
            return interval * amount;
        }
    }
}