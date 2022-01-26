using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthController : MonoBehaviour
{
    private float _health;
    private int _fullHealthCredits;
    private CharacterFaction _faction;
    
    [Header("Health")]
    [FormerlySerializedAs("MaxHealth")] public float maxHealth;

    void Start()
    {
        Health = maxHealth;

        var enemyDescriptor = GetComponent<EnemyDescriptor>();
        if (enemyDescriptor)
        {
            _fullHealthCredits = enemyDescriptor.fullHealthCredits;
            _faction = CharacterFaction.Enemy;
        }
        else
        {
            _faction = CharacterFaction.Tower;
        }
    }

    /// <summary>
    /// Deals damage to this character
    /// </summary>
    /// <param name="amount">the amount of damage to apply</param>
    /// <returns>Character helth</returns>
    /// <exception cref="ArgumentOutOfRangeException">amount can <b>not</b> be 0 or less</exception>
    public float ApplyDamage(float amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException();
        Health -= amount;
        
        CharacterClassifier classifier = new CharacterClassifier(null, null);
        if(GetComponent<EnemyDescriptor>()) classifier = CharacterClassifier.FromEnemy(GetComponent<EnemyDescriptor>());
        if(GetComponent<TowerDescriptor>()) classifier = CharacterClassifier.FromTower(GetComponent<TowerDescriptor>());

        // calculate credits to add to balance from damage
        float creditDropAmount = _fullHealthCredits * (amount / maxHealth);
        GameObject.Find("GameDirector").GetComponent<CreditController>().DepositCredit((long)Math.Round(creditDropAmount), CreditTransactionType.EnemyDamage, classifier);
        
        // report damage
        GameObject.Find("GameDirector").GetComponent<GameStatisticsController>().ReportDamageTransaction(amount, classifier, Health == 0);

        if (Health == 0) Die();
        
        return Health;
    }
    
    /// <summary>
    /// Heal character
    /// </summary>
    /// <param name="amount">the amount to heal the character by</param>
    /// <returns>Character health</returns>
    /// <exception cref="ArgumentOutOfRangeException">amount can <b>not</b> be 0 or less</exception>
    public float Heal(float amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException();
        Health += amount;

        return Health;
    }

    /// <summary>
    /// Destroyes this gameobject
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    public float Health
    {
        get => _health;
        private set => _health = Mathf.Clamp(value, 0, maxHealth);
    }

    /// <summary>
    /// returns a value between 0 and 1
    /// </summary>
    public float HealthPercent => Health / maxHealth;
}
