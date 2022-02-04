using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [Space]
    public string name;
    [Space]
    public List<WaveSection> sections;

    [Header("Modifier per loop")]
    [Range(1, 10)] [Min(1)] public float amountMultiplier = 1;
    [Range(1, 10)] [Min(1)] public float intervalDiminisher = 1;
    
    public float Duration
    {
        get
        {
            float sum = 0;
            
            foreach (var section in sections)
            {
                sum += section.Duration;
            }

            return sum;
        }
    }

    public Wave GetModifiedWave(int loop)
    {
        Wave newWave = new Wave
        {
            name = this.name,
            sections = GetModifiedWaveSections(loop),
            amountMultiplier = this.amountMultiplier,
            intervalDiminisher = this.intervalDiminisher
        };

        return newWave;
    }
    
    public List<WaveSection> GetModifiedWaveSections(int loop)
    {
        List<WaveSection> newSections = new List<WaveSection>();

        foreach (var oldSection in sections)
        {
            newSections.Add(oldSection.GetModifiedWaveSection(loop, amountMultiplier, intervalDiminisher));
        }

        return newSections;
    }
}