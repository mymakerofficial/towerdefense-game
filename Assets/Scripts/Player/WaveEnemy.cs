using System;
using UnityEngine;

[Serializable]
public class WaveEnemy
{
    [Header("Enemy")]
    public GameObject enemy;
    [Min(1)] public int amount;
    [Header("Timing")]
    [Range(0.05f, 50)] [Min(0.001f)] public float interval;
    [Range(0, 50)] [Min(0)] public float startDelay;

    public float Duration
    {
        get
        {
            return interval * amount;
        }
    }
}