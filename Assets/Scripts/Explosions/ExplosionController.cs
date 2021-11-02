using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField]
    public float damage;
    
    [SerializeField]
    public float radius;
    
    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    public void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, 6);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("AddDamage", damage);
        }
        
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(new Vector3(transform.position.x - radius, transform.position.y, transform.position.z), new Vector3(transform.position.x + radius, transform.position.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - radius, transform.position.z), new Vector3(transform.position.x, transform.position.y + radius, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z - radius), new Vector3(transform.position.x, transform.position.y, transform.position.z + radius));
    }
}
