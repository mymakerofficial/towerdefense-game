using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MuzzleOffset
{
    public Vector3 Position;
    public Vector3 Rotation;
}

public class TurretController : MonoBehaviour
{
    public float Range;
    
    public float Damage;
    
    private float cooldown;

    public MuzzleOffset MuzzleOffset;

    public GameObject Head;

    public GameObject Target;

    void Start()
    {
        
    }

    public void Fire()
    {
        GameObject bullet = Instantiate(Resources.Load("Prefabs/Bullets/TurretBullet", typeof(GameObject)) as GameObject, Head.transform.position + MuzzleOffset.Position, Quaternion.Euler(Head.transform.localRotation.eulerAngles + MuzzleOffset.Rotation));
        bullet.SendMessage("Fire");

        this.cooldown -= 0.5f;
    }
    
    public void FixedUpdate()
    {
        Head.transform.LookAt(Target.transform);

        if(cooldown < 1) cooldown += 0.1f;
        
        if(cooldown > 0.9f) this.Fire();
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.DrawLine(new Vector3(transform.position.x - Range, transform.position.y, transform.position.z), new Vector3(transform.position.x + Range, transform.position.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - Range, transform.position.z), new Vector3(transform.position.x, transform.position.y + Range, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z - Range), new Vector3(transform.position.x, transform.position.y, transform.position.z + Range));
    }
}
