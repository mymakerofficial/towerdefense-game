using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;


public class TurretController : MonoBehaviour
{
    private float _cooldown;
    private float _rotation;
    private GameObject _target;

    [Header("Geometry")]
    [FormerlySerializedAs("head")] public GameObject Head;
    [Header("Config")]
    [FormerlySerializedAs("range")] public float Range;
    public float CooldownSec;
    public GameObject BulletGameObject;
    public bool Active;

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
        Active = true;
    }

    public void Fire()
    {
        if (!Active) return;

        GameObject bullet = Instantiate(
            BulletGameObject, 
            transform.position + new Vector3(0, 1.4f, 0), 
            Quaternion.Euler(0, _rotation + 90, 0), 
            GameObject.Find("Bullets").transform
        );
        bullet.SendMessage("Fire");

        Cooldown = CooldownSec;
    }
    
    public void FixedUpdate()
    {
        if (!Active) return;
        
        AutonomousAim();

        // TODO Figgure out a way to have this not have hardcoded values
        Head.transform.rotation = Quaternion.Euler(-90, 0, _rotation + 180);
        
        if (_cooldown > 0)
        {
            _cooldown -= Time.fixedDeltaTime;
        }
        else if(_target)
        {
            Fire();
        }
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
            if (distance > Range) continue; // skip if enemy is outside of range

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
        _rotation = GeneralMath.AngleTowardsPoint2D(Head.transform.position, point);
    }

    public void SetActive(bool value)
    {
        Active = value;
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
