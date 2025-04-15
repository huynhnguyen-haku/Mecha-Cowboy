using UnityEngine;

public class Flamethrower_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;
    private float damageCooldown;
    private float lastTimeDamage;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemy.flamethrowerActive == false)
            return;

        if (Time.time - lastTimeDamage < damageCooldown)
            return;

        I_Damagable damagable = other.GetComponent<I_Damagable>();

        if (damagable != null)
        {
            damagable.TakeDamage();
            lastTimeDamage = Time.time;
            damageCooldown = enemy.flameDamageCooldown;
        }

    }
}

