using UnityEngine;

public class Enemy_Shield : MonoBehaviour, I_Damagable
{
    private Enemy_Melee enemy;
    [SerializeField] private int durability;

    public void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
        durability = enemy.shieldDurability;
    }

    public void ReduceDurability(int damage)
    {
        durability -= damage;

        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); //Enable default chase animation
            gameObject.SetActive(false); //Disable shield
        }
    }

    public void TakeDamage(int damage)
    {
        ReduceDurability(damage);
    }
}
