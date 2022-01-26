using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum CharacterFaction
{
    Tower,
    Enemy
}

public class CharacterClassifier
{
    public CharacterFaction ?Faction { get; }
    public string Name { get; }
    public int Level { get; }

    public CharacterClassifier(CharacterFaction ?faction, string name, int level = 0)
    {
        Faction = faction;
        Name = name;
        Level = level;
    }

    public static CharacterClassifier FromEnemy(EnemyDescriptor descriptor)
    {
        if (descriptor == null) return null;
        return new CharacterClassifier(CharacterFaction.Enemy, descriptor.name);
    }
    
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
    
    public List<CreditTransaction> _creditTransactions;
    public List<DamageTransaction> _damageTransactions;
    
    void Start()
    {
        _gameStateController = gameObject.GetComponent<GameStateController>();
        
        _creditTransactions = new List<CreditTransaction>();
        _damageTransactions = new List<DamageTransaction>();
    }

    public void ReportCreditTransaction(float amount, CreditTransactionType type, CharacterClassifier classifier)
    {
        _creditTransactions.Add(new CreditTransaction(amount, classifier, _gameStateController.GameState, type));
    }   
    
    public void ReportDamageTransaction(float amount, CharacterClassifier classifier, bool fatal)
    {
        _damageTransactions.Add(new DamageTransaction(amount, classifier, fatal));
    }

    public void CalculateStatistics()
    {
        float creditsNegativ = 0;
        float creditsPositiv = 0;

        float damageToEnemies = 0;
        float damageToTowers = 0;

        int towersBought = 0;
        int towersSold = 0;
        int towersUpgraded = 0;

        int enemiesDestroyed = 0;
        int towersDestroyed = 0;

        foreach (var creditTransaction in _creditTransactions)
        {
            if (creditTransaction.Amount >= 0)
            {
                creditsPositiv += creditTransaction.Amount;
            }
            else
            {
                creditsNegativ += creditTransaction.Amount;
            }

            if (creditTransaction.Type == CreditTransactionType.TowerBought)
            {
                towersBought++;
            }
            else if (creditTransaction.Type == CreditTransactionType.TowerSold)
            {
                towersSold++;
            }
            else if (creditTransaction.Type == CreditTransactionType.TowerUpgrade)
            {
                towersUpgraded++;
            }
        }
        
        foreach (var damageTransaction in _damageTransactions)
        {
            if (damageTransaction.ItemClassification.Faction == CharacterFaction.Enemy)
            {
                damageToEnemies += damageTransaction.Amount;

                if (damageTransaction.Fatal) enemiesDestroyed++;
            }
            else
            {
                damageToTowers += damageTransaction.Amount;
                
                if (damageTransaction.Fatal) towersDestroyed++;
            }
        }
        
        Debug.Log($"creditsNegativ {creditsNegativ}");
        Debug.Log($"creditsPositiv {creditsPositiv}");
        Debug.Log($"damageToEnemies {damageToEnemies}");
        Debug.Log($"damageToTowers {damageToTowers}");
        
        Debug.Log($"towersBought {towersBought}");
        Debug.Log($"towersSold {towersSold}");
        Debug.Log($"towersUpgraded {towersUpgraded}");
        
        Debug.Log($"enemiesDestroyed {enemiesDestroyed}");
        Debug.Log($"towersDestroyed {towersDestroyed}");
    }
}
