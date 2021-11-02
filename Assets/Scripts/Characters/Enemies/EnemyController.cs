using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject target;
    private CharacterMovementController movementController;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectsWithTag("Tower")[0];

        movementController = gameObject.GetComponent<CharacterMovementController>();
    }
    
    void Update()
    {
        // calculate the vector3 direction towards the target
        Vector3 dif = (target.transform.position - transform.position);
        
        movementController.Move(dif, 1f);
    }
}
