using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TravelParameters
{
    public float MaxDistance; // how far the bullet can travel
    public float Velocity; // bullet travel speed 
    public bool IsHitscan; // bullet has no travel time
}

[Serializable]
public struct HitParameters
{
    public List<BulletTargetTag> Tags;
    public int MaxHits; // amount of hits before bullet is destroyed
    public float ContactDamage; // the amount of damage to deal to the hit object
}

[Serializable]
public struct BulletTargetTag
{
    public string Name; // tag
    public bool Solid; // will instantly destroy the bullet
    public bool DealDamage; // bullet will deal damage to object
}

public class BulletController : MonoBehaviour
{
    [Header("Bullet Travel")]
    public float MaxTravelDistance; // how far the bullet can travel
    public float TravelVelocity; // bullet travel speed 
    public bool IsHitscan; // bullet has no travel time
    
    [Header("Bullet Hit")]
    public int MaxHits; // amount of hits before bullet is destroyed
    public float DamageOnContact; // the amount of damage to deal to the hit object
    public List<BulletTargetTag> TargetTags;

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
        float stepDistance = TravelVelocity * Time.fixedDeltaTime;

        // hitscan bullets travel entire distance in one frame
        if (IsHitscan) stepDistance = MaxTravelDistance;
        
        // raycast between current possition and next position
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, stepDistance);
        
        
        // loop through all hits
        foreach (var hit in hits)
        {
            // check tags
            foreach (var tag in TargetTags)
            {
                if (!hit.collider.CompareTag(tag.Name)) continue;

                _hitCount++;
                
                if (tag.DealDamage)
                {
                    hit.collider.gameObject.SendMessage("ApplyDamage", DamageOnContact);
                }

                // destroy bullet when hits solid object or to many objects have been hit
                if (_hitCount < MaxHits && !tag.Solid) continue;

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
        if (_distance >= MaxTravelDistance)
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
