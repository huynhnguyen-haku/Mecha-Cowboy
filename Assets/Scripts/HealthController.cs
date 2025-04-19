using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private bool isDead;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage)
    {
        currentHealth -= damage;
    }

    public virtual void IncreaseHealth()
    {
        currentHealth++;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void HealHeath(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Used for enemy
    public bool EnemyShouldDie()
    {
        if (isDead)
        {
            return false;
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            return true;
        }
        return false;
    }

    // Used for player
    public bool PlayerShouldDie()
    {
        return currentHealth <= 0;
    }
}
