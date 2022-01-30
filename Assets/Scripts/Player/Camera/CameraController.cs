using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private InputMaster _controls;
    private GameStateController _gameState;

    private float _absoluteZoom;
    private Vector2 _position;
    private Vector3 _scrollVec;
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

    [Space] public float startZoom;
    [Header("Far")] public float boomFar;
    public float dollyFar;
    public float tiltFar;
    [Header("Near")] public float boomNear;
    public float dollyNear;
    public float tiltNear;
    [Header("Behaviour")] public AnimationCurve zoomCurve;
    public float movementEasing;
    public float movementSpeed;
    public float scrollSpeed;
    public float mouseMovementSpeed;
    [Header("Move Limits")] public Vector2 limitFar;
    public Vector2 limitNear;
    [Header("GameOver")] public Vector3 gameOverPosition;
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
        _controls.Camera.MouseMoveButton.canceled += ctx => { _mouseMove = false; };

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
        Camera cam = GetComponent<Camera>();

        float scrollValue = ctx.ReadValue<Vector2>().y;

        Vector3 zoomVector;
        
        if (scrollValue > 0)
        {
            Ray ray = cam.ScreenPointToRay(_controls.Editor.Point.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit hit, Single.MaxValue, LayerMask.GetMask("Terrain")))
            {
                Vector3 cursorPoint = hit.point;
                Vector3 cameraPos = cam.transform.position;

                zoomVector = (cursorPoint - cameraPos).normalized;
            }
            else
            {
                zoomVector = Vector3.zero;
            }
        }
        else
        {
            zoomVector = cam.transform.forward;
        }

        // TODO: the vector components sometimes become NaN, when scrolling too fast? 
        Vector3 newScrollVec = _scrollVec + (zoomVector * Mathf.Clamp(scrollValue, -1, 1) * scrollSpeed);
        _scrollVec = newScrollVec;
    }

    void FixedUpdate()
    {
        if (_gameState.GameActive)
        {
            // movement with mouse
            if (_mouseMove)
            {
                var mv = (_controls.Camera.MousePosition.ReadValue<Vector2>() - _mouseMoveStartPosition) *
                         mouseMovementSpeed;
                Position = new Vector2(
                    Position.x + Mathf.Clamp(mv.x, -movementSpeed, movementSpeed),
                    Position.y + Mathf.Clamp(mv.y, -movementSpeed, movementSpeed)
                );
            }

            // movement with everything that is not the mouse
            Position += _controls.Camera.Move.ReadValue<Vector2>() * movementSpeed;

         

            float scrollValue = _scrollVec.magnitude;

            if (scrollValue > 0)
            {
                
                // change absolute zoom
                Vector3 camDirection = GetComponent<Camera>().transform.forward;

                Vector3 projected = Vector3.Project(_scrollVec, camDirection);

                if (projected.normalized == camDirection.normalized)
                {
                    scrollValue = -scrollValue;
                }

                float scaledScrollValue = scrollValue * (Time.fixedDeltaTime / 0.016f);

                float positionScale = (Mathf.Clamp(AbsoluteZoom + scaledScrollValue, 0, 1) - AbsoluteZoom) /
                                      scaledScrollValue;
                if (positionScale > 0)
                {
                    Position += new Vector2(_scrollVec.x, _scrollVec.z) * 30 * positionScale;
                }

                AbsoluteZoom += scaledScrollValue;
            }

            // move scroll velocity to 0

            float scrollVecMagnitude = _scrollVec.magnitude;
            scrollVecMagnitude -= scrollVecMagnitude / 10 * (Time.fixedDeltaTime / 0.016f);
            if (scrollVecMagnitude < 0)
            {
                scrollVecMagnitude = 0;
            }
            
            _scrollVec *= scrollVecMagnitude;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 campos = GetComponent<Camera>().transform.position;
        Gizmos.DrawLine(campos, campos + _scrollVec * 100);
    }
}