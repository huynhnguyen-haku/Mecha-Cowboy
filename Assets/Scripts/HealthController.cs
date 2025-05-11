using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    private bool isDead;
    public static bool muteDeathSound = false; // Global flag to mute death sounds
    [SerializeField] private GameObject lowHealthEffect;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        // Check if lowHealthEffect is null before trying to disable it
        if (lowHealthEffect != null)
            lowHealthEffect.SetActive(false);
    }

    public virtual void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        UpdateHeathVFX();
    }

    public virtual void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHeathVFX();
    }

    public void UpdateHealthUI()
    {
        UI.instance.inGameUI.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void UpdateHeathVFX()
    {
        // Return early if lowHealthEffect is null
        if (lowHealthEffect == null)
            return;


        // Enable VFX if health is below 50%
        if (currentHealth < maxHealth * 0.5f)
        {
            if (!lowHealthEffect.activeSelf)
                lowHealthEffect.SetActive(true);
        }
        // Disable VFX if health is 50% or above
        else
        {
            if (lowHealthEffect.activeSelf)
                lowHealthEffect.SetActive(false);
        }
    }

    // Used for enemy
    public bool EnemyShouldDie()
    {
        if (isDead)
            return false;

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
        // Only disable the low health effect if the player is dead
        if (currentHealth <= 0 && lowHealthEffect != null)
            lowHealthEffect.SetActive(false);
        
        return currentHealth <= 0;
    }


    public virtual void SetHealthToZero()
    {
        currentHealth = 0;
        isDead = true;

        if (TryGetComponent(out Enemy enemy))
        {
            enemy.Die();
        }
    }
}
