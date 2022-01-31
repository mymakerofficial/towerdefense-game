using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public struct Placement
{
    public GameObject tower;
    public Vector3 position;
    public Quaternion rotation;
    public bool available;
}

[Serializable]
public struct ProtectedZone
{
    public Vector3 position;
    public float radius;
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
    
    private GameObject _gameDirector;

    private Placement _placement;

    private GameObject _dummy;
    private GameObject _circle;
    private PlacementMode _mode = PlacementMode.Idle;

    [Space]
    public GameObject parrent;
    [Space]
    public float distanceToMapObjects;
    [Space]
    public List<ProtectedZone> protectedZones;

    /// <summary>
    /// Initialize the basics
    /// </summary>
    void Awake()
    {
        _camera = Camera.main; // get active camera i guess
        _controls = new InputMaster();
        
        _gameDirector = GameObject.Find("GameDirector");

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

        _placement.tower = obj;

        float scale = 100;
        if (_placement.tower.GetComponent<TowerDescriptor>()) scale = _placement.tower.GetComponent<TowerDescriptor>().placementRadius * 100;
        _circle.transform.localScale = new Vector3(scale, scale, scale);
        _circle.SetActive(true);
        
        CreateDummy();
        _mode = PlacementMode.Position;
    }

    public void CancelPlacement()
    {
        _mode = PlacementMode.Idle;
        Reset();
    }

    /// <summary>
    /// Creates a dummy object of the object to place
    /// </summary>
    private void CreateDummy()
    {
        // create dummy object
        _dummy = Instantiate(_placement.tower, transform);

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
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        if(!_placement.available || _mode == PlacementMode.Idle) return;
        
        _mode++; // next step

        // skip rotation if not needed
        if (_mode == PlacementMode.Rotation && !_placement.tower.GetComponent<TowerDescriptor>().requiresRotation)
        {
            _mode++;
        }

        if (_mode == PlacementMode.Done) Place(); // place tower
    }
    
    /// <summary>
    /// Previous step. Also resets when mode goes to idle
    /// </summary>
    private void Back()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        _mode--; // previous step

        if (_mode == PlacementMode.Idle) Reset(); // go to sleep
    }

    /// <summary>
    /// Places the object and restarts the sequence
    /// </summary>
    private void Place()
    {
        long requiredCredits = _placement.tower.GetComponent<TowerDescriptor>().cost;

        if (GameObject.Find("GameDirector").GetComponent<CreditController>().CurrentCredits >= requiredCredits)
        {
            GameObject.Find("GameDirector").GetComponent<CreditController>().WithdrawCredit(requiredCredits, CreditTransactionType.TowerBought, CharacterClassifier.FromTower(_placement.tower.GetComponent<TowerDescriptor>()));
            
            // Create real object
            Instantiate(_placement.tower, _placement.position, _placement.rotation, parrent.transform);
        }

        // Place next one
        StartPlacement(_placement.tower);
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
        _placement.tower = null;
        _placement.position = Vector3.zero;
        _placement.rotation = new Quaternion();
        _placement.available = true;
    }
    
    void FixedUpdate()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
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
        _placement.position = RaycastCursorPosition();
            
        // check if position is to close to other towers
        _placement.available = true;
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower"); // get every tower in the scene

        foreach (GameObject tower in towers)
        {
            float distance =  Vector3.Distance(tower.transform.position, _placement.position);
            // if distance is smaller then combined radii 
            if (distance < tower.GetComponent<TowerDescriptor>().placementRadius + _placement.tower.GetComponent<TowerDescriptor>().placementRadius)
            {
                _placement.available = false;
                break; // no need to check others
            }
        }
        
        // check for collision with map objects 
        Collider[] hitColliders = Physics.OverlapSphere(_placement.position, _placement.tower.GetComponent<TowerDescriptor>().placementRadius + distanceToMapObjects);
        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject; // get gameobject from collider

            if (obj.layer == LayerMask.NameToLayer("Map"))
            {
                _placement.available = false;
                break; // no need to check others
            }
        }
        
        // check for protected zones
        foreach (var zone in protectedZones)
        {
            if (Vector3.Distance(zone.position, _placement.position) <
                zone.radius + _placement.tower.GetComponent<TowerDescriptor>().placementRadius)
            {
                _placement.available = false;
                break; // no need to check others
            }
        }
        

        // set positions
        _dummy.transform.position = _placement.position;
        _circle.transform.position = _placement.position + new Vector3(0, 0.1f, 0);

        // change circle texture
        // TODO Optimize this!
        if (_placement.available)
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
        Vector2 dir2 = (new Vector2(_placement.position.x, _placement.position.z) -
                        new Vector2(hitPoint.x, hitPoint.z)).normalized;

        float angle = (float)(Math.Atan2(dir2.y, -dir2.x) * (180 / Math.PI) + 90);
            
        _placement.rotation = Quaternion.Euler(0, angle, 0);
            
        _dummy.transform.rotation = _placement.rotation;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var zone in protectedZones)
        {
            Gizmos.DrawWireSphere(zone.position, zone.radius);
        }
    }
}
