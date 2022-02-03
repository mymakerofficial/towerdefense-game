using System;
using System.Collections.Generic;

[Serializable]
public class WaveSection
{
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

            return top;
        }
    }
}