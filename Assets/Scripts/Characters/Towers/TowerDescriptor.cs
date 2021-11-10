using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDescriptor : MonoBehaviour
{
    public string name;
    public string description;
    public int level;
    public float placementRadius;
    public GameObject nextUpgrade;
    public string levelDescription;
    public int cost;
    public int levelUpgradeCost;
}
