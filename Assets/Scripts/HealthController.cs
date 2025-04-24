using System;
using Unity.VisualScripting;
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

    public void UpdateHealthUI()
    {
        UI.instance.inGameUI.UpdateHealthUI(currentHealth, maxHealth);
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

    public virtual void SetHealthToZero()
    {
        currentHealth = 0;
        isDead = true;

        // Chuyển sang DeadState nếu kẻ địch có EnemyStateMachine
        if (TryGetComponent(out Enemy enemy))
        {
            if (enemy.stateMachine != null)
            {
                // Sử dụng DeadState đã có thay vì tạo mới
                if (enemy is Enemy_Range rangeEnemy)
                {
                    enemy.stateMachine.ChangeState(rangeEnemy.deadState);
                }
                else if (enemy is Enemy_Melee meleeEnemy)
                {
                    enemy.stateMachine.ChangeState(meleeEnemy.deadState);
                }
                else if (enemy is Enemy_Boss bossEnemy)
                {
                    enemy.stateMachine.ChangeState(bossEnemy.deadState);
                }
            }
        }
    }
}
