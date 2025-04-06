using UnityEngine;

public class Enemy_Shield : MonoBehaviour
{
    private Enemy_Melee enemy;
    [SerializeField] private int durability;

    public void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
    }

    public void ReduceDurability()
    {
        durability--;
        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); //Enable default chase animation
            Destroy(gameObject);
        }
    }
}
