using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct TravelParameters
{
    [FormerlySerializedAs("MaxDistance")] public float maxDistance; // how far the bullet can travel
    [FormerlySerializedAs("Velocity")] public float velocity; // bullet travel speed 
    [FormerlySerializedAs("IsHitscan")] public bool isHitscan; // bullet has no travel time
}

[Serializable]
public struct HitParameters
{
    [FormerlySerializedAs("Tags")] public List<BulletTargetTag> tags;
    [FormerlySerializedAs("MaxHits")] public int maxHits; // amount of hits before bullet is destroyed
    [FormerlySerializedAs("ContactDamage")] public float contactDamage; // the amount of damage to deal to the hit object
}

[Serializable]
public struct BulletTargetTag
{
    [FormerlySerializedAs("Name")] public string name; // tag
    [FormerlySerializedAs("Solid")] public bool solid; // will instantly destroy the bullet
    [FormerlySerializedAs("DealDamage")] public bool dealDamage; // bullet will deal damage to object
}

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

    private bool _active;
    private Vector3 _origin;
    private float _distance;
    private int _hitCount;

    public void Fire()
    {
        _origin = transform.position;
        _active = true;
    }

    private void FixedUpdate()
    {
        // dont do anything when not active
        if(!_active) return;
        
        // calculate step distance with delta time
        float stepDistance = travelVelocity * Time.fixedDeltaTime;

        // hitscan bullets travel entire distance in one frame
        if (isHitscan) stepDistance = maxTravelDistance;
        
        // raycast between current possition and next position
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
                Disolve();
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
            Disolve();
        }
    }

    private void Disolve()
    {
        /*
         *  Insert code that needs to run before destruction here
         */
        
        Destroy(gameObject);
    }
}
