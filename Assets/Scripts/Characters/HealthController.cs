using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private float health;
    [SerializeField]
    public float MaxHealth;
    
    void Start()
    {
        health = MaxHealth;
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
        this.health -= amount;

        if (this.health <= 0) this.Die();
        
        return Math.Max(this.health, 0);
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
        this.health += amount;

        if (this.health > this.MaxHealth) this.health = this.MaxHealth;

        return this.health;
    }

    /// <summary>
    /// Destroyes this gameobject
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    public float Health => this.health;

    /// <summary>
    /// returns a value between 0 and 1
    /// </summary>
    public float HealthPercent => this.health / this.MaxHealth;
}
