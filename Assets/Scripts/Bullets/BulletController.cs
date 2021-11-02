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
    public List<Tag> Tags;
    public int MaxHits; // amount of hits before bullet is destroyed
    public float ContactDamage; // the amount of damage to deal to the hit object
}

[Serializable]
public struct Tag
{
    public string Name; // tag
    public bool Solid; // will instantly destroy the bullet
    public bool DealDamage; // bullet will deal damage to object
}

public class BulletController : MonoBehaviour
{
    [SerializeField] public TravelParameters BulletTravel;
    [SerializeField] public HitParameters BulletHit;

    private bool active;
    private Vector3 origin;
    private float distance;
    private int hitCount;

    public void Fire()
    {
        origin = transform.position;
        active = true;
    }

    private void FixedUpdate()
    {
        // dont do anything when not active
        if(!active) return;

        if (BulletTravel.IsHitscan)
        {
            BulletTravel.Velocity = BulletTravel.MaxDistance;
        }
        
        // raycast between current possition and next position
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, BulletTravel.Velocity);
        
        
        // loop through all hits
        foreach (var hit in hits)
        {
            // check tags
            foreach (var tag in BulletHit.Tags)
            {
                if (!hit.collider.CompareTag(tag.Name)) continue;

                hitCount++;
                
                if (tag.DealDamage)
                {
                    hit.collider.gameObject.SendMessage("ApplyDamage", BulletHit.ContactDamage);
                }

                // destroy bullet when hits solid object or to many objects have been hit
                if (hitCount < BulletHit.MaxHits) continue; 
                if (!tag.Solid) continue;

                transform.position = hit.point;
                Disolve();
                break;
            }
        }
        
        // move to next step
        transform.position += transform.forward * BulletTravel.Velocity;
        
        // update distance
        distance += BulletTravel.Velocity;

        // if distance to far destroy
        if (distance >= BulletTravel.MaxDistance)
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
