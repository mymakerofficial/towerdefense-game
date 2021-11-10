using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : MonoBehaviour
{
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    void Update()
    {
        
        target = GameObject.FindGameObjectsWithTag("Tower")[0];
        // calculate the vector3 direction towards the target
        Vector3 dif = (target.transform.position - transform.position);

        transform.position += dif.normalized * Time.deltaTime;
    }
}
