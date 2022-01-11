using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


public class TurretController : MonoBehaviour
{
    private float _cooldown;
    private float _rotation;
    private GameObject _target;

    public GameObject head;
    public float range;
    public bool autonomous;
    public bool active;

    public float Cooldown
    {
        get => _cooldown;
        private set => _cooldown = Mathf.Clamp(value, 0, 1);
    }
    
    public float Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    void Start()
    {
        autonomous = true;
        _cooldown = 1;
        active = true;
    }

    public void Fire()
    {
        if (!active) return;
        if (Cooldown == 0) return;
        
        GameObject bullet = Instantiate(
            Resources.Load<GameObject>("Prefabs/Bullets/Bullet"), 
            transform.position + new Vector3(0, 1.4f, 0), 
            Quaternion.Euler(0, _rotation + 90, 0), 
            GameObject.Find("Bullets").transform
        );
        bullet.SendMessage("Fire");

        Cooldown -= 0.03f;
    }
    
    public void FixedUpdate()
    {
        if (!active) return;
        
        if (autonomous)
        {
            AutonomousAim();

            if (Cooldown > 0.9f && _target) Fire();
        }

        Cooldown += 0.1f * Time.fixedDeltaTime;

        // TODO Figgure out a way to have this not have hardcoded values
        head.transform.rotation = Quaternion.Euler(-90, 0, _rotation + 180);
    }

    /// <summary>
    /// Rotate to closest enemy
    /// </summary>
    public void AutonomousAim()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float closestDistance = Single.MaxValue;
        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance > closestDistance) continue; // skip if enemy is not closest
            if (distance > range) continue; // skip if enemy is outside of range

            closest = enemy;
            closestDistance = distance;
        }
        
        _target = closest;
        
        if(_target) RotateTowards(_target.transform.position);
    }

    /// <summary>
    /// Rotate towars a point
    /// </summary>
    public void RotateTowards(Vector3 point)
    {
        _rotation = GeneralMath.AngleTowardsPoint2D(head.transform.position, point);
    }

    public void SetActive(bool value)
    {
        active = value;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_target != null)
        {
            Gizmos.DrawLine(transform.position, _target.transform.position);
            Gizmos.DrawSphere(_target.transform.position, 0.2f);
        }
    }
}
