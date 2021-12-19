using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    private float _health;
    [SerializeField]
    public float MaxHealth;
    
    private int _fullHealthCredits;
    
    void Start()
    {
        Health = MaxHealth;

        var enemyDescriptor = GetComponent<EnemyDescriptor>();
        if (enemyDescriptor)
        {
            _fullHealthCredits = enemyDescriptor.fullHealthCredits;
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

        // calculate credits to add to balance from damage
        float creditDropAmount = _fullHealthCredits * (amount / MaxHealth);
        GameObject.Find("GameDirector").SendMessage("DepositCredit", (long)creditDropAmount);

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
        private set => _health = Mathf.Clamp(value, 0, MaxHealth);
    }

    /// <summary>
    /// returns a value between 0 and 1
    /// </summary>
    public float HealthPercent => Health / MaxHealth;
}
