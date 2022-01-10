using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private InputMaster _controls;

    private float _absoluteZoom;
    private Vector2 _position;
    private float _scrollVel;
    private bool _mouseMove;
    private Vector2 _mouseMoveStartPosition;

    public float startZoom;
    public float boomFar;
    public float dollyFar;
    public float tiltFar;
    public float boomNear;
    public float dollyNear;
    public float tiltNear;
    public AnimationCurve zoomCurve;
    public float movementEasing;
    public float movementSpeed;
    public float scrollSpeed;
    public float mouseMovementSpeed;


    private void Awake()
    {
        _controls = new InputMaster();
        
        // scroll... duh'
        _controls.Camera.Scroll.performed += ctx => Scroll(ctx);
        
        // activate mouse movement and store mouse starting position
        _controls.Camera.MouseMoveButton.performed += ctx =>
        {
            _mouseMove = true;
            _mouseMoveStartPosition = _controls.Camera.MousePosition.ReadValue<Vector2>();
        };
        
        // stop mousemovement
        _controls.Camera.MouseMoveButton.canceled += ctx =>
        {
            _mouseMove = false;
        };

        absoluteZoom = startZoom;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    private float absoluteZoom
    {
        get => _absoluteZoom;
        set => _absoluteZoom = Mathf.Clamp(value, 0, 1);
    }

    private float zoom => zoomCurve.Evaluate(absoluteZoom);

    private Vector3 positionCurrent
    {
        get
        {
            float boom = Mathf.Lerp(boomNear, boomFar, zoom);
            float dolly = Mathf.Lerp(dollyNear, dollyFar, zoom);
            return new Vector3(_position.x, boom, _position.y + dolly);
        }
    }
    
    private Vector3 rotationCurrent
    {
        get
        {
            float x = Mathf.Lerp(tiltNear, tiltFar, zoom);
            return new Vector3(x, 0, 0);
        }
    }

    void Scroll(InputAction.CallbackContext ctx)
    {
        _scrollVel += -Mathf.Clamp(ctx.ReadValue<Vector2>().y, -1, 1) * scrollSpeed;
    }
    
    void FixedUpdate()
    {
        // movement with mouse
        if (_mouseMove == true)
        {
            _position += (_controls.Camera.MousePosition.ReadValue<Vector2>() - _mouseMoveStartPosition) * mouseMovementSpeed;
        }
        
        // movement with everything that is not the mouse
        _position += _controls.Camera.Move.ReadValue<Vector2>() * movementSpeed;

        transform.position += (positionCurrent - transform.position) * movementEasing;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
                                              (rotationCurrent - transform.rotation.eulerAngles) * movementEasing);

        absoluteZoom += _scrollVel * (Time.fixedDeltaTime / 0.016f);

        _scrollVel -= _scrollVel / 10 * (Time.fixedDeltaTime / 0.016f);
    }
}
