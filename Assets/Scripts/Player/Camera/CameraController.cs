using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private InputMaster _controls;
    private GameStateController _gameState;

    private float _absoluteZoom;
    private Vector2 _position;
    private float _scrollVel;
    private bool _mouseMove;
    private Vector2 _mouseMoveStartPosition;
    private float _distanceToStrongholdOnGameOver;

    private Vector2 Position
    {
        get => _position;
        set
        {
            float limitX = Mathf.Lerp(limitNear.x, limitFar.x, Zoom);
            float limitY = Mathf.Lerp(limitNear.y, limitFar.y, Zoom);

            _position = new Vector2(
                Mathf.Clamp(value.x, -limitX, limitX),
                Mathf.Clamp(value.y, -limitY, limitY)
            );
        }
    }

    [Space]
    public float startZoom;
    [Header("Far")]
    public float boomFar;
    public float dollyFar;
    public float tiltFar;
    [Header("Near")]
    public float boomNear;
    public float dollyNear;
    public float tiltNear;
    [Header("Behaviour")]
    public AnimationCurve zoomCurve;
    public float movementEasing;
    public float movementSpeed;
    public float scrollSpeed;
    public float mouseMovementSpeed;
    [Header("Move Limits")] 
    public Vector2 limitFar;
    public Vector2 limitNear;
    [Header("GameOver")] 
    public Vector3 gameOverPosition;
    public Vector3 gameOverRotation;
    public float gameOverTransitionEasing;
    

    /// <summary>
    /// Is the camera beeing moved with mouse
    /// </summary>
    public bool MouseMoveActive => _mouseMove;
    /// <summary>
    /// Start position for mouse movement
    /// </summary>
    public Vector2 MouseMoveStartPosition => _mouseMoveStartPosition;

    private void Awake()
    {
        _gameState = GameObject.Find("GameDirector").GetComponent<GameStateController>();
        
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

        AbsoluteZoom = startZoom;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    /// <summary>
    /// Zoom value before curve 
    /// </summary>
    private float AbsoluteZoom
    {
        get => _absoluteZoom;
        set => _absoluteZoom = Mathf.Clamp(value, 0, 1);
    }

    /// <summary>
    /// Actual zoom value after curve
    /// </summary>
    public float Zoom => zoomCurve.Evaluate(AbsoluteZoom);

    /// <summary>
    /// Calculated position of camera
    /// </summary>
    private Vector3 PositionCurrent
    {
        get
        {
            float boom = Mathf.Lerp(boomNear, boomFar, Zoom);
            float dolly = Mathf.Lerp(dollyNear, dollyFar, Zoom);
            return new Vector3(Position.x, boom, Position.y + dolly);
        }
    }
    
    /// <summary>
    /// Calculated rotation of camera
    /// </summary>
    private Vector3 RotationCurrent
    {
        get
        {
            float x = Mathf.Lerp(tiltNear, tiltFar, Zoom);
            return new Vector3(x, 0, 0);
        }
    }

    void Scroll(InputAction.CallbackContext ctx)
    {
        // increase scroll velocity in direction of scroll
        _scrollVel += -Mathf.Clamp(ctx.ReadValue<Vector2>().y, -1, 1) * scrollSpeed;
    }
    
    void FixedUpdate()
    {
        if (_gameState.GameActive)
        {
            // movement with mouse
            if (_mouseMove)
            {
                var mv = (_controls.Camera.MousePosition.ReadValue<Vector2>() - _mouseMoveStartPosition) * mouseMovementSpeed;
                Position = new Vector2(
                    Position.x + Mathf.Clamp(mv.x, -movementSpeed, movementSpeed),
                    Position.y + Mathf.Clamp(mv.y, -movementSpeed, movementSpeed)
                );
            }
        
            // movement with everything that is not the mouse
            Position += _controls.Camera.Move.ReadValue<Vector2>() * movementSpeed;
            
            // change absolute zoom
            AbsoluteZoom += _scrollVel * (Time.fixedDeltaTime / 0.016f);

            // move scroll velocity to 0
            _scrollVel -= _scrollVel / 10 * (Time.fixedDeltaTime / 0.016f);
        }

        Vector3 targetPosition = PositionCurrent;
        Vector3 targetRotation = RotationCurrent;
        float easing = movementEasing;

        // overwrite position, rotation and easing when gameover
        if (_gameState.GameState == GameState.GameOver)
        {
            if (_distanceToStrongholdOnGameOver == 0)
            {
                // save distance to target position
                _distanceToStrongholdOnGameOver = Vector3.Distance(transform.position, gameOverPosition);
            }
            targetPosition = gameOverPosition;
            targetRotation = gameOverRotation;
            easing = gameOverTransitionEasing / (_distanceToStrongholdOnGameOver / 100);
        }

        // smooth translation for position
        transform.position += (targetPosition - transform.position) * easing;

        // smooth translation for rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
                                              (targetRotation - transform.rotation.eulerAngles) * easing);
    }
}
