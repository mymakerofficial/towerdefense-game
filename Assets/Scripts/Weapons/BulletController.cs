using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Travel")]
    [FormerlySerializedAs("MaxTravelDistance")] public float maxTravelDistance; // how far the bullet can travel
    [FormerlySerializedAs("TravelVelocity")] public float travelVelocity; // bullet travel speed 
    [FormerlySerializedAs("IsHitscan")] public bool isHitscan; // bullet has no travel time
    
    [Header("Bullet Hit")]
    [FormerlySerializedAs("MaxHits")] public int maxHits; // amount of hits before bullet is destroyed
    [FormerlySerializedAs("DamageOnContact")] public float damageOnContact; // the amount of damage to deal to the hit object
    [FormerlySerializedAs("TargetTags")] public List<BulletTargetTag> targetTags;

    [Header("Visuals")] 
    public GameObject particleSystem;

    private bool _active;
    private Vector3 _origin;
    private float _distance;
    private int _hitCount;

    private GameObject _gameDirector;

    void Start()
    {
        _gameDirector = GameObject.Find("GameDirector");
    }

    public void Fire()
    {
        _origin = transform.position;
        _active = true;
    }

    private void FixedUpdate()
    {
        if(_gameDirector.GetComponent<GameStateController>().Paused) return; // dont do anything when game is paused
        
        // dont do anything when not active
        if(!_active) return;
        
        // calculate step distance with delta time
        float stepDistance = travelVelocity * Time.fixedDeltaTime;

        // hitscan bullets travel entire distance in one frame
        if (isHitscan) stepDistance = maxTravelDistance;
        
        // raycast between current position and next position
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, stepDistance);
        
        
        // loop through all hits
        foreach (var hit in hits)
        {
            // check tags
            foreach (var tag in targetTags)
            {
                if (!hit.collider.CompareTag(tag.name)) continue;

                _hitCount++;
                
                if (tag.dealDamage)
                {
                    hit.collider.gameObject.SendMessage("ApplyDamage", damageOnContact);
                }

                // destroy bullet when hits solid object or to many objects have been hit
                if (_hitCount < maxHits && !tag.solid) continue;

                transform.position = hit.point;
                StartCoroutine(DestroyAfterTime());
                break;
            }
        }
        
        // move to next step
        transform.position += transform.forward * stepDistance;
        
        // update distance
        _distance += stepDistance;

        // if distance to far destroy
        if (_distance >= maxTravelDistance)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    /// <summary>
    /// Destroy this game object after 3 seconds to let particle animation play
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyAfterTime()
    {
        travelVelocity = 0;
        if(particleSystem != null) particleSystem.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
