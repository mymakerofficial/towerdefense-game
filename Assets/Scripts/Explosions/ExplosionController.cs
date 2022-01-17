using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float Damage;
    public float Radius;
    public List<string> DealDamageTo;
    public float DelaySec;
    
    void Start()
    {
        Explode();
    }

    public void Explode()
    {
        // get all colliders in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject; // get gameobject from collider

            if (DealDamageTo.Contains(obj.tag)) // check if tag is in list of tags to apply damage to
            {
                obj.SendMessage("ApplyDamage", Damage);
            }
        }
        
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
