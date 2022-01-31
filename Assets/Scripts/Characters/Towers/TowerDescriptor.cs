using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDescriptor : MonoBehaviour
{
    [Header("Description")]
    public string name;
    public string description;
    [Header("Placement")]
    public float placementRadius;
    public bool requiresRotation;
    [Header("Level")]
    public int level;
    public GameObject nextUpgrade;
    public string levelDescription;
    [Header("Credits")]
    public int cost;
    public int levelUpgradeCost;
}
