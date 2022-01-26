using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterFaction
{
    Tower,
    Enemy
}

[Serializable]
public abstract class Transaction
{
    private float _amount;
    private GameState _activeGameState;

    public float Amount => _amount;
    public GameState ActiveGameState => _activeGameState;

    protected Transaction(float amount, GameState activeGameState)
    {
        _amount = amount;
        _activeGameState = activeGameState;
    }
}

[Serializable]
public class CreditTransaction : Transaction
{
    private CreditTransactionType _type;

    public CreditTransactionType Type => _type;
    
    public CreditTransaction(float amount, GameState activeGameState, CreditTransactionType type) : base(amount, activeGameState)
    {
        _type = type;
    }
}

[Serializable]
public class DamageTransaction : Transaction
{
    private CharacterFaction _recieverFaction;
    private bool _fatal;

    public CharacterFaction RecieverFaction => _recieverFaction;
    public bool Fatal => _fatal;
    
    public DamageTransaction(float amount, GameState activeGameState, CharacterFaction reciever, bool fatal) : base(amount, activeGameState)
    {
        _recieverFaction = reciever;
        _fatal = fatal;
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

    public void ReportCreditTransaction(float amount, CreditTransactionType type)
    {
        _creditTransactions.Add(new CreditTransaction(amount, _gameStateController.GameState, type));
    }   
    
    public void ReportDamageTransaction(float amount, CharacterFaction faction, bool fatal)
    {
        _damageTransactions.Add(new DamageTransaction(amount, _gameStateController.GameState, faction, fatal));
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

            switch (creditTransaction.Type)
            {
                case CreditTransactionType.TowerBought:
                    towersBought++;
                    break;
                case CreditTransactionType.TowerSold:
                    towersSold++;
                    break;
                case CreditTransactionType.TowerUpgrade:
                    towersUpgraded++;
                    break;
            }
        }
        
        foreach (var damageTransaction in _damageTransactions)
        {
            if (damageTransaction.RecieverFaction == CharacterFaction.Enemy)
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
