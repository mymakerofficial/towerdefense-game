using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public struct HitParameters
{
    [FormerlySerializedAs("Tags")] public List<BulletTargetTag> tags;
    [FormerlySerializedAs("MaxHits")] public int maxHits; // amount of hits before bullet is destroyed
    [FormerlySerializedAs("ContactDamage")] public float contactDamage; // the amount of damage to deal to the hit object
}