using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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