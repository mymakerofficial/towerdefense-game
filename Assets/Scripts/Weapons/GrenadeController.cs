using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    [Space]
    public float angle;
    public float minDistance;
    public float radius;
    [Space] 
    public GameObject explosion;

    private Vector3 _target;
    private Vector3 _spawnPosition;
    
    private GameObject _gameDirector;
    private Vector3 _pausedVelocity;
    private Vector3 _pausedAngularVelocity;
    private bool _isPaused;

    void Start()
    {
        _gameDirector = GameObject.Find("GameDirector");
        _spawnPosition = transform.position;
    }

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

    public void Fire(GameObject target)
    {
        _target = target.transform.position;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(CalculateVelocity(target.transform.position) * rigidbody.mass, ForceMode.Impulse);
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation, transform.parent).SendMessage("Fire");
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!_gameDirector.GetComponent<GameStateController>().Paused)
        {
            if (_isPaused)
            {
                GetComponent<Rigidbody>().WakeUp();
                GetComponent<Rigidbody>().AddForce( _pausedVelocity, ForceMode.Impulse );
                GetComponent<Rigidbody>().AddTorque( _pausedAngularVelocity, ForceMode.Impulse );
                _isPaused = false;
            }
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            if (hitColliders.Length > 0 && Vector3.Distance(transform.position, _spawnPosition) > minDistance)
            {
                Explode();
            }
        }
        else if(!_isPaused)
        {
            _pausedVelocity = GetComponent<Rigidbody>().velocity;
            _pausedAngularVelocity = GetComponent<Rigidbody>().angularVelocity;
            GetComponent<Rigidbody>().Sleep();
            _isPaused = true;
        }
    }
}
