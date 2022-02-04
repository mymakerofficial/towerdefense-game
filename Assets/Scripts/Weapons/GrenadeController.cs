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
    private bool _isStopped;

    void Start()
    {
        _gameDirector = GameObject.Find("GameDirector");
        _spawnPosition = transform.position;
    }

    /// <summary>
    /// Calculates the required velocity to hit target
    /// </summary>
    /// <param name="target"></param>
    /// <returns>velocity to hit target</returns>
    private Vector3 CalculateVelocity(Vector3 target)
    {
        // i couldnt figure out how to get this to work but this calculation i found on the unity forum works
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
        // apply force to rigidbody
        rigidbody.AddForce(CalculateVelocity(target.transform.position) * rigidbody.mass, ForceMode.Impulse);
    }

    void Explode()
    {
        if (_isStopped) return; // dont explode multiple times
        // create explosion
        Instantiate(explosion, transform.position, transform.rotation, transform.parent).SendMessage("Fire");
        // stop
        _isStopped = true;
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

    private void FixedUpdate()
    {
        if (!_gameDirector.GetComponent<GameStateController>().Paused || _isStopped)
        {
            if (_isPaused)
            {
                // unpause
                GetComponent<Rigidbody>().WakeUp(); // unfreeze rigidbody
                GetComponent<Rigidbody>().AddForce( _pausedVelocity, ForceMode.Impulse ); // add stored force to rigidbody
                GetComponent<Rigidbody>().AddTorque( _pausedAngularVelocity, ForceMode.Impulse ); // add stored torque to rigidbody
                _isPaused = false;
            }
            
            // check if is colliding with something
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            if (hitColliders.Length > 0 && Vector3.Distance(transform.position, _spawnPosition) > minDistance)
            {
                // boom
                Explode();
            }
        }
        else if(!_isPaused)
        {
            _pausedVelocity = GetComponent<Rigidbody>().velocity; // store rigidbody force
            _pausedAngularVelocity = GetComponent<Rigidbody>().angularVelocity; // store rigidbody torque
            GetComponent<Rigidbody>().Sleep(); // freeze rigidbody
            _isPaused = true;
        }
    }
}
