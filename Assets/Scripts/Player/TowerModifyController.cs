using System;
using UnityEngine;

public class TowerModifyController : MonoBehaviour
{
    private GameObject _gameDirector;
    
    private Camera _camera;
    private InputMaster _controls;

    private GameObject _circle;
    private GameObject _circleRange;

    private GameObject _closeTower;
    private GameObject _selectedTower;

    private GameObject _healthBarOuter;
    private GameObject _healthBarInner;

    [Space]
    public float selectRadiusFactor = 1.2f;
    [Space] 
    public GameObject towerPlacer;
    
    [Header("UI")]
    public GameObject infoPanel;
    public GameObject infoPanelName;
    public GameObject infoPanelLevel;
    public GameObject infoPanelDescription;
    public GameObject infoPanelUpgrade;
    public GameObject infoPanelSell;
    public GameObject infoPanelUpgradeCredits;
    public GameObject infoPanelSellCredits;
    [Space]
    public GameObject healthBar;
    [Space] 
    public float healthBarOffset;
    public float healthBarPadding;

    [Header("Textures")] 
    public Texture circleOutline;
    public Texture circleSelected;
    public Texture circleRange;

    public GameObject SelectedTower => _selectedTower;
    
    void Awake()
    {
        _camera = Camera.main; // get active camera i guess
        _controls = new InputMaster();
        _gameDirector = GameObject.Find("GameDirector");

        _healthBarOuter = healthBar.transform.GetChild(0).gameObject;
        _healthBarInner = _healthBarOuter.transform.GetChild(0).gameObject;
        
        // create circle
        _circle = Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlacementCircle"), gameObject.transform);
        _circle.SetActive(false);
        
        _circleRange = Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlacementCircle"), gameObject.transform);
        _circleRange.GetComponent<Renderer>().material.SetTexture("_MainTex", circleRange);
        _circleRange.SetActive(false);

        // register control events
        _controls.Editor.Next.performed += _ => Select();
        _controls.Editor.Back.performed += _ => UnSelect();
    }
    
    private void OnEnable() => _controls.Enable();
    private void OnDestroy() => _controls.Disable();
    
    void FixedUpdate()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return; // dont do stuff when game is paused

        if (towerPlacer.GetComponent<TowerPlacementController>().PlacementMode != PlacementMode.Idle || _gameDirector.GetComponent<GameStateController>().GameState == GameState.GameOver)
        {
            // unselect if tower placer is active or game over
            UnSelect();
        }else
        {
            FindClosest();
        }

        if (_selectedTower != null)
        {
            // set selection circle for selected tower
            _circle.SetActive(true);
            float scalePlacement = _selectedTower.GetComponent<TowerDescriptor>().placementRadius * 100;
            _circle.transform.localScale = new Vector3(scalePlacement, scalePlacement, scalePlacement);
            _circle.transform.position = _selectedTower.transform.position  + new Vector3(0, 0.1f, 0);;

            // show range if availabe
            if (_selectedTower.GetComponent<TurretController>())
            {
                // show range circle
                _circleRange.SetActive(true);
                // set range circle transform
                float scaleRange = _selectedTower.GetComponent<TurretController>().range * 100;
                _circleRange.transform.localScale = new Vector3(scaleRange, scaleRange, scaleRange);
                _circleRange.transform.position = _selectedTower.transform.position  + new Vector3(0, 0.1f, 0);;
            }
        }
        else if (_closeTower != null)
        {
            // show outline on nearest tower
            _circle.SetActive(true);
            // set circle transform
            float scale = _closeTower.GetComponent<TowerDescriptor>().placementRadius * 100;
            _circle.transform.localScale = new Vector3(scale, scale, scale);
            _circle.transform.position = _closeTower.transform.position  + new Vector3(0, 0.1f, 0);;
            
            // show range if availabe
            if (_closeTower.GetComponent<TurretController>())
            {
                // show range circle
                _circleRange.SetActive(true);
                // set range circle transform
                float scaleRange = _closeTower.GetComponent<TurretController>().range * 100;
                _circleRange.transform.localScale = new Vector3(scaleRange, scaleRange, scaleRange);
                _circleRange.transform.position = _closeTower.transform.position  + new Vector3(0, 0.1f, 0);;
            }
        }
        else
        {
            // hide all circles
            _circle.SetActive(false);
            _circleRange.SetActive(false);
        }
        
        GameObject focusedTower = _selectedTower != null ? _selectedTower : _closeTower;

        if (focusedTower != null)
        {
            // show tower health bar
            
            // get point on screen for focused tower position
            Vector3 point = _camera.WorldToScreenPoint(focusedTower.transform.position);
            
            healthBar.SetActive(true);
            healthBar.transform.position = point + new Vector3(0, healthBarOffset, 0);
            // calculate healthbar width
            float maxWidth = _healthBarOuter.GetComponent<RectTransform>().sizeDelta.x -
                             healthBarPadding * 2;
            float width = maxWidth * focusedTower.GetComponent<HealthController>().HealthPercent;
            // set health bar
            _healthBarInner.GetComponent<RectTransform>().sizeDelta = new Vector2(width, -healthBarPadding * 2);
        }
        else
        {
            // hide health bar
            healthBar.SetActive(false);
        }

        // update menu information
        // credit amounts need to be updated real time
        if (_selectedTower)
        {
            infoPanelUpgradeCredits.GetComponent<UnityEngine.UI.Text>().text = $"-{UpgradeCreditAmount}";
            infoPanelSellCredits.GetComponent<UnityEngine.UI.Text>().text = $"+{SellCreditAmount}";
            
            infoPanelUpgrade.SetActive(_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade != null);
        }
    }

    private void Select()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return; // dont select when game is paused
        
        if(_closeTower != null)
        {
            // select tower
            _selectedTower = _closeTower;
            _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", circleSelected);
            
            // show info panel
            infoPanel.SetActive(true);
            
            // set static information on info panel
            infoPanelName.GetComponent<UnityEngine.UI.Text>().text =
                _closeTower.GetComponent<TowerDescriptor>().name;
            infoPanelLevel.GetComponent<UnityEngine.UI.Text>().text = $"Lvl. {_closeTower.GetComponent<TowerDescriptor>().level}";
            infoPanelDescription.GetComponent<UnityEngine.UI.Text>().text =
                _closeTower.GetComponent<TowerDescriptor>().description;
        }
    }

    public void UnSelect()
    {
        if (_gameDirector.GetComponent<GameStateController>().Paused) return; // dont unselect when game is paused
        
        // clear selected tower
        _selectedTower = null;
        _circle.GetComponent<Renderer>().material.SetTexture("_MainTex", circleOutline);
        infoPanel.SetActive(false);
    }

    private int SellCreditAmount
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
        // deposit credits
        GameObject.Find("GameDirector").GetComponent<CreditController>().DepositCredit(
            SellCreditAmount, 
            CreditTransactionType.TowerSold, 
            CharacterClassifier.FromTower(_selectedTower.GetComponent<TowerDescriptor>())
        );
        UnSelect();
    }

    private int UpgradeCreditAmount
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
        if (_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade) // check for next upgrade
        {
            if (GameObject.Find("GameDirector").GetComponent<CreditController>()
                .CheckSufficientCredits(UpgradeCreditAmount)) // check if player has enough credits to perform transaction
            {
                
                // withdraw credits
                GameObject.Find("GameDirector").GetComponent<CreditController>().WithdrawCredit(
                    UpgradeCreditAmount,
                    CreditTransactionType.TowerUpgrade,
                    CharacterClassifier.FromTower(_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade.GetComponent<TowerDescriptor>())
                );
                
                // create new tower
                GameObject newTower = Instantiate(_selectedTower.GetComponent<TowerDescriptor>().nextUpgrade, _selectedTower.transform.parent);
                // set transform of new tower
                newTower.transform.position = _selectedTower.transform.position;
                newTower.transform.rotation = _selectedTower.transform.rotation;
            
                // destroy old tower
                Destroy(_selectedTower);
                
                // unselect tower
                UnSelect();
            }
        }
    }

    private void FindClosest()
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
