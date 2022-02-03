using System;
using UnityEngine.Serialization;

[Serializable]
public struct TravelParameters
{
    [FormerlySerializedAs("MaxDistance")] public float maxDistance; // how far the bullet can travel
    [FormerlySerializedAs("Velocity")] public float velocity; // bullet travel speed 
    [FormerlySerializedAs("IsHitscan")] public bool isHitscan; // bullet has no travel time
}