using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [Space]
    public float angle;
    public float triggerDistance;
    [Space] 
    public GameObject explosion;

    private Vector3 _target;

    private Vector3 CalculateVelocity(Vector3 target)
    {
        // https://answers.unity.com/questions/148399/shooting-a-cannonball.html
        var dir = target - transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        var dist = dir.magnitude ;  // get horizontal distance
        var a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
        // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));

        return vel * dir.normalized;
    }

    void Fire(GameObject target)
    {
        _target = target.transform.position;
        
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(CalculateVelocity(target.transform.position) * rigidbody.mass, ForceMode.Impulse);
    }

    void Explode()
    {
        Instantiate(explosion, transform.parent).SendMessage("Fire");
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            if (Vector3.Distance(transform.position, _target) < triggerDistance) Explode();
        }
    }
}
