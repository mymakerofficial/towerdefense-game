using System;
using UnityEngine.Serialization;

[Serializable]
public struct BulletTargetTag
{
    [FormerlySerializedAs("Name")] public string name; // tag
    [FormerlySerializedAs("Solid")] public bool solid; // will instantly destroy the bullet
    [FormerlySerializedAs("DealDamage")] public bool dealDamage; // bullet will deal damage to object
}