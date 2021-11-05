using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDescriptor : MonoBehaviour
{
    public string Name;
    public int Level;
    public float PlacementRadius;
    
    private void OnDrawGizmosSelected(){
        UnityEditor.Handles.DrawWireDisc(transform.position ,Vector3.up, PlacementRadius);
    }
}
