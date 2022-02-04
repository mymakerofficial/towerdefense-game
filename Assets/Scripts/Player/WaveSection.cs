using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveSection
{
    [Range(0, 50)] [Min(0)] public float startDelay;  
    [Space]
    public List<WaveEnemy> enemies;
    
    public float Duration
    {
        get
        {
            float top = 0;
            
            foreach (var enemy in enemies)
            {
                float duration = enemy.Duration + enemy.startDelay;

                if (duration > top)
                {
                    top = duration;
                }
            }

            return top + startDelay;
        }
    }

    public WaveSection GetModifiedWaveSection(int loop, float amountMultiplier, float intervalDiminisher)
    {
        WaveSection newWaveSection = new WaveSection();

        newWaveSection.enemies = GetModifiedEnemies(loop, amountMultiplier, intervalDiminisher);

        return newWaveSection;
    }
    
    public List<WaveEnemy> GetModifiedEnemies(int loop, float amountMultiplier, float intervalDiminisher)
    {
        if (loop == 0)
        {
            // dont modify if there is no loop
            return enemies;
        }
        
        List<WaveEnemy> newEnemies = new List<WaveEnemy>();

        foreach (var oldEnemy in enemies)
        {
            WaveEnemy newEnemy = new WaveEnemy
            {
                enemy = oldEnemy.enemy,
                amount = (int)(oldEnemy.amount * (amountMultiplier * loop)),
                interval = oldEnemy.interval / (intervalDiminisher * loop),
                startDelay = oldEnemy.startDelay / (intervalDiminisher * loop)
            };
            
            newEnemies.Add(newEnemy);
        }

        return newEnemies;
    }
}