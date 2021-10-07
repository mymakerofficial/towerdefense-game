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
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    private Vector2 InputMove => _controls.Jeff.Movement.ReadValue<Vector2>();
    
    void FixedUpdate()
    {
        var force = new Vector3(InputMove.x, 0, InputMove.y);
        gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }
}
