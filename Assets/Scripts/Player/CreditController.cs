using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public enum CreditTransactionType
{
    Unknown,
    EnemyDamage,
    TowerBought,
    TowerUpgrade,
    TowerSold
}

public class CreditController : MonoBehaviour
{
    private long _credit;
    [Space]
    public long startCredit;

    public long CurrentCredits => _credit;
    public void Start()
    {
        _credit = startCredit;
    }

    /// <summary>
    /// add the amount to the credit 
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public long DepositCredit(long amount, CreditTransactionType transactionType = CreditTransactionType.Unknown, [CanBeNull] CharacterClassifier classifier = null)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException($"can not deposit negative amount");
        }
        
        gameObject.GetComponent<GameStatisticsController>().ReportCreditTransaction(amount, transactionType, classifier);

        _credit += amount;
        return _credit;
    }

    /// <summary>
    /// Subtract the credit by the amount
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InsufficientCreditException"></exception>
    public long WithdrawCredit(long amount, CreditTransactionType transactionType = CreditTransactionType.Unknown, [CanBeNull] CharacterClassifier classifier = null)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException($"can not deposit negative amount");
        }

        if (_credit - amount < 0)
        {
            throw new InsufficientCreditException("not enough credit",amount,_credit);
        }
        
        gameObject.GetComponent<GameStatisticsController>().ReportCreditTransaction(-amount, transactionType, classifier);

        _credit -= amount;
        return _credit;
    }

    /// <summary>
    /// Check if you have enough Credits
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CheckSufficientCredits(long amount)
    {
        return _credit - amount >= 0;
    }
}

public class InsufficientCreditException : Exception
{
    /// <summary>
    /// Credits Needed for operation 
    /// </summary>
    public long CreditNeeded { get; }
    /// <summary>
    /// given Credits 
    /// </summary>
    public long CreditProvided { get; }
    

    public InsufficientCreditException(string message,Exception innerException, long creditNeeded, long creditProvided) : base(message,innerException)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
    public InsufficientCreditException(string message, long creditNeeded, long creditProvided) : base(message)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
    
    public InsufficientCreditException(long creditNeeded, long creditProvided)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
}
