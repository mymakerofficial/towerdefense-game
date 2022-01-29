using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerModifyController : MonoBehaviour
{
    private GameObject _gameDirector;
    
    private Camera _camera;
    private InputMaster _controls;

    private GameObject _circle;

    private GameObject _closeTower;
    private GameObject _selectedTower;

    [Space]
    [FormerlySerializedAs("SelectRadiusFactor")] public float selectRadiusFactor = 1.5f;
    [Space]
    [FormerlySerializedAs("UiElement")] public GameObject uiElement;

    public GameObject SelectedTower => _selectedTower;
    
    void Awake()
    {
        _camera = Camera.main; // get active camera i guess
        _controls = new InputMaster();
        _gameDirector = GameObject.Find("GameDirector");
        
        // create circle
        _circle = Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlacementCircle"), gameObject.transform);
        float scale = 100;
        _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("Textures/UI/SelectCircle"));
        _circle.transform.localScale = new Vector3(100, 100, 100);
        _circle.SetActive(false);

        // register control events
        _controls.Editor.Next.performed += _ => Select();
        _controls.Editor.Back.performed += _ => UnSelect();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    void FixedUpdate()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        if (!_selectedTower) FindCloseset();

        if (_selectedTower)
        {
            _circle.SetActive(true);
            float scale = _closeTower.GetComponent<TowerDescriptor>().placementRadius * 100;
            _circle.transform.localScale = new Vector3(scale, scale, scale);
            _circle.transform.position = _selectedTower.transform.position  + new Vector3(0, 0.1f, 0);;
        }
        else if (_closeTower)
        {
            _circle.SetActive(true);
            float scale = _closeTower.GetComponent<TowerDescriptor>().placementRadius * 100;
            _circle.transform.localScale = new Vector3(scale, scale, scale);
            _circle.transform.position = _closeTower.transform.position  + new Vector3(0, 0.1f, 0);;
        }
        else
        {
            _circle.SetActive(false);
        }
    }

    private void Select()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        if(_closeTower != null)
        {
            _selectedTower = _closeTower;
            _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("Textures/UI/PlacementCircle"));
            uiElement.SetActive(true);
            uiElement.transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text =
                _closeTower.GetComponent<TowerDescriptor>().name;
            uiElement.transform.Find("Description").GetComponent<UnityEngine.UI.Text>().text =
                _closeTower.GetComponent<TowerDescriptor>().description;
            uiElement.transform.Find("Upgrade").transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text =
                $"Upgrade -{UpgradeCreditAmount}c";
            uiElement.transform.Find("Sell").transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text =
                $"Sell +{SellCreditAmount}c";
        }
    }

    private void UnSelect()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return;
        
        _selectedTower = null;
        _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("Textures/UI/SelectCircle"));
        uiElement.SetActive(false);
    }

    public int SellCreditAmount
    {
        get
        {
            if (_selectedTower == null) return 0;
            return (int)(_selectedTower.GetComponent<TowerDescriptor>().cost *
                         _selectedTower.GetComponent<HealthController>().HealthPercent);
        }
    }

    public void Sell()
    {
        Destroy(_selectedTower);
        GameObject.Find("GameDirector").GetComponent<CreditController>().DepositCredit(SellCreditAmount, CreditTransactionType.TowerSold, CharacterClassifier.FromTower(_selectedTower.GetComponent<TowerDescriptor>()));
        UnSelect();
    }

    public int UpgradeCreditAmount
    {
        get
        {
            if (_selectedTower == null) return 0;
            if (_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade == null) return 0;
            return _selectedTower.GetComponent<TowerDescriptor>().nextUpgrade.GetComponent<TowerDescriptor>().levelUpgradeCost;
        }
    }
    
    public void Upgrade()
    {
        if (_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade)
        {
            if (GameObject.Find("GameDirector").GetComponent<CreditController>()
                .CheckSufficientCredits(UpgradeCreditAmount))
            {
                GameObject.Find("GameDirector").GetComponent<CreditController>().WithdrawCredit(UpgradeCreditAmount, CreditTransactionType.TowerUpgrade, CharacterClassifier.FromTower(_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade.GetComponent<TowerDescriptor>()));
                
                GameObject newTower = Instantiate(_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade, _selectedTower.transform.parent);
                newTower.transform.position = _selectedTower.transform.position;
                newTower.transform.rotation = _selectedTower.transform.rotation;
            
                Destroy(_selectedTower);
                UnSelect();
            }
        }
    }

    private void FindCloseset()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower"); // get every tower in the scene
        
        // return if there are no towers in the scene
        if(towers.Length == 0) return;

        Vector3 point = RaycastCursorPosition();
        
        // look for closest tower
        float closestDistance = float.MaxValue;
        GameObject closest = null;
        foreach (GameObject tower in towers)
        {
            float distance =  Vector3.Distance(tower.transform.position, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = tower;
            }
        }
        
        // return if there was nothing found (this should be impossible but better save then sorry)
        if(closest == null) return;

        // set if in radius
        if (closestDistance < closest.GetComponent<TowerDescriptor>().placementRadius * selectRadiusFactor)
        {
            _closeTower = closest;
        }
        else
        {
            _closeTower = null;
        }
    }
    
    /// <summary>
    /// Raycasts a point from the mouse position
    /// </summary>
    /// <returns>Raycasted point</returns>
    private Vector3 RaycastCursorPosition() //TODO This is a repeat (TowerPlacementController)
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
