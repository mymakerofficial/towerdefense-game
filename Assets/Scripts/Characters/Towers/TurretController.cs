using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;


public enum Axis
{
    X,
    Y,
    Z
}

public class TurretController : MonoBehaviour
{
    private GameObject _gameDirector;
    
    private float _cooldown;
    private float _headRotation;
    private GameObject _target;

    [Header("Geometry")] 
    public GameObject head;
    public Vector3 headRotationOffset;
    public Axis rotateAxis;

    [Header("Config")] 
    public float range;

    [Header("Attack")]
    [FormerlySerializedAs("FireGameObject")] public GameObject fireGameObject;
    public FireCallOptions fireCallOptions;
    [Space]
    public Vector3 fireOffset;
    [Space]
    [FormerlySerializedAs("AttackCooldownSec")] public float attackCooldownSec;

    void Start()
    {
        _gameDirector = GameObject.Find("GameDirector");
        
        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }

    public void Attack()
    {
        GameObject bullet = Instantiate(
            fireGameObject, 
            transform.position + fireOffset, 
            Quaternion.Euler(0, GeneralMath.AngleTowardsPoint2D(transform.position, _target.transform.position) + 90, 0), 
            GameObject.Find("Bullets").transform
        );
        
        switch (fireCallOptions)
        {
            case FireCallOptions.CallFire:
                bullet.SendMessage("Fire");
                break;
            case FireCallOptions.SendTarget:
                bullet.SendMessage("Fire", _target);
                break;
        }
    }
    
    public void FixedUpdate()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        if (_cooldown > 0)
        {
            _cooldown -= Time.fixedDeltaTime;
        }
        else if(_target)
        {
            _cooldown = attackCooldownSec;
        
            Attack();
        }
    
        if (_target != null && head != null)
        {
            _headRotation += (GeneralMath.AngleTowardsPoint2D(head.transform.position, _target.transform.position) - _headRotation) / 10;
            head.transform.rotation = Quaternion.Euler(headRotationOffset.x + (rotateAxis == Axis.X ? _headRotation : 0), headRotationOffset.y + (rotateAxis == Axis.Y ? _headRotation : 0), headRotationOffset.z + (rotateAxis == Axis.Z ? _headRotation : 0));
        }
    }

    private void UpdateTarget()
    {
        _target = FindClosest();
    }

    /// <summary>
    /// Rotate to closest enemy
    /// </summary>
    public GameObject FindClosest()
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

        return closest;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position + fireOffset, 0.2f);
    }
}
