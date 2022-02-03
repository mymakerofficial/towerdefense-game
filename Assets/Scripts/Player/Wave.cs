using System;
using System.Collections.Generic;

[Serializable]
public class Wave
{
    public List<WaveSection> sections;
    
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
}