using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemyDespawnerController : MonoBehaviour
{
    public GameObject enemyParent;
    public float effectDistance;

    private void FixedUpdate()
    {
        foreach (Transform enemyTransform in enemyParent.transform) // loop through all enemies
        {
            if (Vector3.Distance(transform.position, enemyTransform.position) < effectDistance) // check if enemy is in range
            {
                Destroy(enemyTransform.gameObject);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, effectDistance);
    }
}
