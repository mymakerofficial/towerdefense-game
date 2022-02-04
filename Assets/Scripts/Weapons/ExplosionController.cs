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
            GameObject obj = hitCollider.gameObject; // get game object from collider

            if (dealDamageTo.Contains(obj.tag)) // check if tag is in list of tags to apply damage to
            {
                obj.SendMessage("ApplyDamage", damage);
            }
        }

        StartCoroutine(DestroyAfterTime());
    }

    /// <summary>
    /// Destroy this game object after 3 seconds to let particle animation play
    /// </summary>
    /// <returns></returns>
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
