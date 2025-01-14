using UnityEngine;
using UnityEngine.Serialization;

public class StrongholdController : MonoBehaviour
{
    [Header("Enemies")]
    [FormerlySerializedAs("enemyParrent")] public GameObject enemyParent;
    [FormerlySerializedAs("affectDistance")] public float effectDistance;
    [Header("Health")]
    public int fullHealth;
    
    private int _health;

    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            if (value >= 0)
            {
                _health = value;
            }
        }
    }

    public float HealthPercent =>  (float)Health / (float)fullHealth;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Health = fullHealth;
    }

    private void FixedUpdate()
    {
        foreach (Transform enemyTransform in enemyParent.transform) // loop through all enemies
        {
            if (Vector3.Distance(transform.position, enemyTransform.position) < effectDistance) // check if enemy is in range
            {
                // destroy enemy
                Destroy(enemyTransform.gameObject);
                // decrease health by 1
                Health--;
                
                if(Health != 0) Debug.Log($"Stronghold Health: {HealthPercent*100}%");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, effectDistance);
    }
}
