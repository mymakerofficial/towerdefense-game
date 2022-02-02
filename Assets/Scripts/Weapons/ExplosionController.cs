using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplosionController : MonoBehaviour
{
    [FormerlySerializedAs("Damage")] public float damage;
    [FormerlySerializedAs("Radius")] public float radius;
    [FormerlySerializedAs("DealDamageTo")] public List<string> dealDamageTo;

    public void Fire()
    {
        // get all colliders in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject; // get gameobject from collider

            if (dealDamageTo.Contains(obj.tag)) // check if tag is in list of tags to apply damage to
            {
                obj.SendMessage("ApplyDamage", damage);
            }
        }

        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
