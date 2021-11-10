using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

/*
 *      What is Jeff?
 *      Jeff is a objekt made for testing and refference
 */

public class JeffContoller : MonoBehaviour
{
    private InputMaster _controls;
    
    void Awake()
    {
        _controls = new InputMaster();

        _controls.Jeff.Shoot.performed += _ => Shoot();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    private Vector2 InputMove => _controls.Jeff.Movement.ReadValue<Vector2>();
    private float InputRotate => _controls.Jeff.Rotation.ReadValue<float>();
    
    void FixedUpdate()
    {
        var force = new Vector3(InputMove.x, 0, InputMove.y);
        var rb = gameObject.GetComponent<Rigidbody>();
        rb.AddRelativeForce(force, ForceMode.Impulse);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0, InputRotate * 100, 0) * Time.fixedDeltaTime));
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/Bullet"), transform.position + new Vector3(0, 0, 1), transform.rotation);
        bullet.SendMessage("Fire");
        
    }
}
