using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassifier
{
    /// <summary>
    /// If character is either tower or enemy
    /// </summary>
    public CharacterFaction ?Faction { get; }
    public string Name { get; }
    public int Level { get; }

    public CharacterClassifier(CharacterFaction ?faction, string name, int level = 0)
    {
        Faction = faction;
        Name = name;
        Level = level;
    }

    /// <summary>
    /// Returns CharacterClassifier from EnemyDescriptor
    /// </summary>
    /// <param name="descriptor">EnemyDescriptor to generate CharacterClassifier from</param>
    /// <returns>new CharacterClassifier</returns>
    public static CharacterClassifier FromEnemy(EnemyDescriptor descriptor)
    {
        if (descriptor == null) return null;
        return new CharacterClassifier(CharacterFaction.Enemy, descriptor.name);
    }
    
    /// <summary>
    /// Returns CharacterClassifier from TowerDescriptor
    /// </summary>
    /// <param name="descriptor">TowerDescriptor to generate CharacterClassifier from</param>
    /// <returns>new CharacterClassifier</returns>
    public static CharacterClassifier FromTower(TowerDescriptor descriptor)
    {
        if (descriptor == null) return null;
        return new CharacterClassifier(CharacterFaction.Tower, descriptor.name, descriptor.level);
    }
}

[Serializable]
public abstract class Transaction
{
    public float Amount { get; }
    [CanBeNull] public CharacterClassifier ItemClassification { get; }

    protected Transaction(float amount, [CanBeNull] CharacterClassifier classifier = null)
    {
        Amount = amount;
        ItemClassification = classifier;
    }
}

[Serializable]
public class CreditTransaction : Transaction
{
    public CreditTransactionType Type { get; }

    public GameState ActiveGameState { get; }

    public CreditTransaction(float amount, [CanBeNull] CharacterClassifier classifier, GameState activeGameState, CreditTransactionType type) : base(amount, classifier)
    {
        Type = type;
        ActiveGameState = activeGameState;
    }
}

[Serializable]
public class DamageTransaction : Transaction
{
    public bool Fatal { get; }

    public DamageTransaction(float amount, [CanBeNull] CharacterClassifier classifier, bool fatal) : base(amount, classifier)
    {
        Fatal = fatal;
    }
}


public class GameStatisticsController : MonoBehaviour
{
    private GameStateController _gameStateController;
    
    private List<CreditTransaction> _creditTransactions;
    private List<DamageTransaction> _damageTransactions;
    
    private float _creditsSpend;
    private float _creditsEarned;

    private float _damageToEnemies;
    private float _damageToTowers;

    private int _towersBought;
    private int _towersSold;
    private int _towersUpgraded;

    private int _enemiesDestroyed;
    private int _towersDestroyed;

    private int _wavesSurvived;
    private int _totalCredits;

    [Header("UI")] 
    public GameObject creditsSpendUiElement;
    public GameObject creditsEarnedUiElement;
    public GameObject towersBoughtUiElement;
    public GameObject damageDealtUiElement;
    public GameObject damageRecievedUiElement;
    public GameObject enemiesDestroyedUiElement;
    public GameObject towersDestroyedUiElement;
    public GameObject wavesSurvivedUiElement;
    public GameObject totalCreditsUiElement;

    void Start()
    {
        _gameStateController = gameObject.GetComponent<GameStateController>();
        
        _creditTransactions = new List<CreditTransaction>();
        _damageTransactions = new List<DamageTransaction>();

        _wavesSurvived = 0;
    }

    /// <summary>
    /// Reset all values
    /// </summary>
    public void Reset() => Start();

    public void ReportCreditTransaction(float amount, CreditTransactionType type, CharacterClassifier classifier)
    {
        _creditTransactions.Add(new CreditTransaction(amount, classifier, _gameStateController.GameState, type));
    }   
    
    public void ReportDamageTransaction(float amount, CharacterClassifier classifier, bool fatal)
    {
        _damageTransactions.Add(new DamageTransaction(amount, classifier, fatal));
    }

    public void ReportWaveEnd()
    {
        _wavesSurvived++;
    }

    public void CalculateStatistics()
    {
        float creditsSpend = 0;
        float creditsEarned = 0;

        float damageToEnemies = 0;
        float damageToTowers = 0;

        int towersBought = 0;
        int towersSold = 0;
        int towersUpgraded = 0;

        int enemiesDestroyed = 0;
        int towersDestroyed = 0;

        foreach (var creditTransaction in _creditTransactions)
        {

            if (creditTransaction.Type == CreditTransactionType.TowerBought)
            {
                // count credits spend on tower
                creditsSpend += -creditTransaction.Amount;
                
                // count tower bought
                towersBought++;
            }
            else if (creditTransaction.Type == CreditTransactionType.TowerSold)
            {
                // tower sales dont count towards credits earned
                
                // count tower sold
                towersSold++;
            }
            else if (creditTransaction.Type == CreditTransactionType.TowerUpgrade)
            {
                // count credits spend on tower upgrades
                creditsSpend += -creditTransaction.Amount;
                
                // count tower upgrade
                towersUpgraded++;
            }else if (creditTransaction.Type == CreditTransactionType.EnemyDamage)
            {
                // count credits earned from enemy damage
                creditsEarned += creditTransaction.Amount;
            }
        }
        
        foreach (var damageTransaction in _damageTransactions)
        {
            if (damageTransaction.ItemClassification.Faction == CharacterFaction.Enemy)
            {
                // count damage dealt to enemies
                damageToEnemies += damageTransaction.Amount;

                // count enemies destroyed
                if (damageTransaction.Fatal) enemiesDestroyed++;
            }
            else
            {
                // count damage dealt to towers
                damageToTowers += damageTransaction.Amount;
                
                // count tower destroyed
                if (damageTransaction.Fatal) towersDestroyed++;
            }
        }

        _creditsSpend = creditsSpend;
        _creditsEarned = creditsEarned;
        _damageToEnemies = damageToEnemies;
        _damageToTowers = damageToTowers;
        _towersBought = towersBought;
        _towersSold = towersSold;
        _towersUpgraded = towersUpgraded;
        _enemiesDestroyed = enemiesDestroyed;
        _towersDestroyed = towersDestroyed;

        _totalCredits = (int)GetComponent<CreditController>().CurrentCredits;
        
        Debug.Log($"creditsSpend {creditsSpend}");
        Debug.Log($"creditsEarned {creditsEarned}");
        Debug.Log($"damageToEnemies {damageToEnemies}");
        Debug.Log($"damageToTowers {damageToTowers}");
        
        Debug.Log($"towersBought {towersBought}");
        Debug.Log($"towersSold {towersSold}");
        Debug.Log($"towersUpgraded {towersUpgraded}");
        
        Debug.Log($"enemiesDestroyed {enemiesDestroyed}");
        Debug.Log($"towersDestroyed {towersDestroyed}");
        
        ShowStats();
    }

    /// <summary>
    /// Set values on Ui
    /// </summary>
    void ShowStats()
    {
        creditsSpendUiElement.GetComponent<Text>().text = $"{_creditsSpend}c";
        creditsEarnedUiElement.GetComponent<Text>().text = $"{_creditsEarned}c";
        towersBoughtUiElement.GetComponent<Text>().text = $"{_towersBought}";
        damageDealtUiElement.GetComponent<Text>().text = $"{_damageToEnemies}";
        damageRecievedUiElement.GetComponent<Text>().text = $"{_damageToTowers}";
        enemiesDestroyedUiElement.GetComponent<Text>().text = $"{_enemiesDestroyed}";
        towersDestroyedUiElement.GetComponent<Text>().text = $"{_towersDestroyed}";
        wavesSurvivedUiElement.GetComponent<Text>().text = $"{_wavesSurvived}";
        totalCreditsUiElement.GetComponent<Text>().text = $"{_totalCredits}c";
    }
}
