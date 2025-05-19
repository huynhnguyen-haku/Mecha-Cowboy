using UnityEngine;

public class Enemy_Shield : MonoBehaviour, I_Damagable
{
    private Enemy_Melee enemy; // Reference to the parent enemy
    [SerializeField] private int durability; // Current durability of the shield

    #region Unity Methods
    public void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
        durability = enemy.shieldDurability;
    }
    #endregion

    #region Damage Handling
    // Reduce the shield's durability when taking damage
    public void ReduceDurability(int damage)
    {
        durability -= damage;

        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); // Enable default chase animation
            gameObject.SetActive(false); // Disable shield
        }
    }

    // Implement I_Damagable interface to take damage
    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
    #endregion
}