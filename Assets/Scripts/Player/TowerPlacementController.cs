using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public struct Placement
{
    public GameObject Object;
    public Vector3 Position;
    public Quaternion Rotation;
    public bool Available;
}

public enum PlacementMode
{
    Idle,
    Position,
    Rotation,
    Done
}

public class TowerPlacementController : MonoBehaviour
{
    private Camera _camera;
    private InputMaster _controls;

    private Placement _placement;

    private GameObject _dummy;
    private GameObject _circle;
    private PlacementMode _mode = PlacementMode.Idle;

    public GameObject parrent;

    /// <summary>
    /// Initialize the basics
    /// </summary>
    void Awake()
    {
        _camera = Camera.main; // get active camera i guess
        _controls = new InputMaster();

        // register control events
        _controls.Editor.Next.performed += _ => Next();
        _controls.Editor.Back.performed += _ => Back();
        
        // create circle
        _circle = Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlacementCircle"), gameObject.transform);
        
        Reset();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();

    /// <summary>
    /// This starts the placment sequence
    /// </summary>
    /// <param name="obj">The object (tower) you want to place</param>
    public void StartPlacement(GameObject obj)
    {
        Reset();

        _placement.Object = obj;

        float scale = 100;
        if (_placement.Object.GetComponent<TowerDescriptor>()) scale = _placement.Object.GetComponent<TowerDescriptor>().placementRadius * 100;
        _circle.transform.localScale = new Vector3(scale, scale, scale);
        _circle.SetActive(true);
        
        CreateDummy();
        _mode = PlacementMode.Position;
    }

    /// <summary>
    /// Creates a dummy object of the object to place
    /// </summary>
    private void CreateDummy()
    {
        // create dummy object
        _dummy = Instantiate(_placement.Object, transform);

        _dummy.tag = "Untagged"; // dummy can not have tower tag
        
        // Remove all components
        foreach (var comp in _dummy.GetComponents<Component>())
        {
            //Don't remove the Transform component
            if (comp is Transform) continue;
            
            DestroyImmediate(comp);
        }
    }

    /// <summary>
    /// Next step. Also places when mode goes to done
    /// </summary>
    private void Next()
    {
        if(!_placement.Available || _mode == PlacementMode.Idle) return;
        
        _mode++; // next step

        if (_mode == PlacementMode.Done) Place(); // place tower
    }
    
    /// <summary>
    /// Previous step. Also resets when mode goes to idle
    /// </summary>
    private void Back()
    {
        _mode--; // previous step

        if (_mode == PlacementMode.Idle) Reset(); // go to sleep
    }

    /// <summary>
    /// Places the object and restarts the sequence
    /// </summary>
    private void Place()
    {
        long requiredCredits = _placement.Object.GetComponent<TowerDescriptor>().cost;

        if (GameObject.Find("GameDirector").GetComponent<CreditController>().CurrentCredits >= requiredCredits)
        {
            GameObject.Find("GameDirector").SendMessage("WithdrawCredit", requiredCredits);
            
            // Create real object
            Instantiate(_placement.Object, _placement.Position, _placement.Rotation, parrent.transform);
        }

        // Place next one
        StartPlacement(_placement.Object);
    }

    /// <summary>
    /// Resets all the things back to null
    /// </summary>
    public void Reset()
    {
        // delete dummy if exists
        if(_dummy) Destroy(_dummy);
        
        // hide circle
        _circle.SetActive(false);
        
        // reset mode
        _mode = PlacementMode.Idle;

        // reset all values
        _placement.Object = null;
        _placement.Position = Vector3.zero;
        _placement.Rotation = new Quaternion();
        _placement.Available = true;
    }
    
    void FixedUpdate()
    {
        // TODO currently only supports mouse input. It should support other input methods too.
        
        if(_mode == PlacementMode.Position)
        {
            UpdatePosition();
        }

        if (_mode == PlacementMode.Rotation)
        {
            UpdateRotation();
        }
    }

    /// <summary>
    /// Updates the position
    /// </summary>
    private void UpdatePosition()
    {
        _placement.Position = RaycastCursorPosition();
            
        // check if position is to close to other towers
        _placement.Available = true;
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower"); // get every tower in the scene

        foreach (GameObject tower in towers)
        {
            float distance =  Vector3.Distance(tower.transform.position, _placement.Position);
            // if distance is smaller then combined radii 
            if (distance < tower.GetComponent<TowerDescriptor>().placementRadius + _placement.Object.GetComponent<TowerDescriptor>().placementRadius)
            {
                _placement.Available = false;
                break; // no need to check others
            }
        }
            
        // set positions
        _dummy.transform.position = _placement.Position;
        _circle.transform.position = _placement.Position + new Vector3(0, 0.1f, 0);

        // change circle texture
        // TODO Optimize this!
        if (_placement.Available)
        {
            _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("Textures/UI/PlacementCircle"));
        }
        else
        {
            _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("Textures/UI/PlacementCircleRed"));
        }
    }

    /// <summary>
    /// Updates the rotation
    /// </summary>
    private void UpdateRotation()
    {
        Vector3 hitPoint = RaycastCursorPosition();
        
        // calculate angle
        Vector2 dir2 = (new Vector2(_placement.Position.x, _placement.Position.z) -
                        new Vector2(hitPoint.x, hitPoint.z)).normalized;

        float angle = (float)(Math.Atan2(dir2.y, -dir2.x) * (180 / Math.PI) + 90);
            
        _placement.Rotation = Quaternion.Euler(0, angle, 0);
            
        _dummy.transform.rotation = _placement.Rotation;
    }

    /// <summary>
    /// Raycasts a point from the mouse position
    /// </summary>
    /// <returns>Raycasted point</returns>
    private Vector3 RaycastCursorPosition()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(_controls.Editor.Point.ReadValue<Vector2>());//Mouse.current.position.ReadValue()
        if (Physics.Raycast(ray, out hit, Single.MaxValue, LayerMask.GetMask("Terrain")))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
