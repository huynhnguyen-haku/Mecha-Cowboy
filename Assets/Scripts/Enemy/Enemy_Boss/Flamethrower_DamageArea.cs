using System.Collections;
using UnityEngine;

public class Flamethrower_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;
    private CapsuleCollider capsuleCollider;

    private float damageCooldown; // Cooldown between damage ticks
    private float lastTimeDamage; // Last time damage was dealt
    private int flameDamage; // Damage dealt by the flamethrower

    #region Unity Methods
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Called when another collider stays within the trigger
    // This is used to deal damage to player and other enemies
    private void OnTriggerStay(Collider other)
    {
        if (enemy.flamethrowerActive == false)
            return;

        if (Time.time - lastTimeDamage < damageCooldown)
            return;

        I_Damagable damagable = other.GetComponent<I_Damagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage);
            lastTimeDamage = Time.time;
            damageCooldown = enemy.flameDamageCooldown;
        }
    }
    #endregion

    #region Collider Expansion
    // Start expanding the collider of the flamethrower damage area
    public void StartExpandingCollider()
    {
        StartCoroutine(ExpandCollider());
    }

    // Coroutine to expand the collider over time
    private IEnumerator ExpandCollider()
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        float startHeight = 1.2f;
        float targetHeight = 6f;

        float startCenterZ = -1.9f;
        float targetCenterZ = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp for height and center
            capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y, Mathf.Lerp(startCenterZ, targetCenterZ, t));

            yield return null;
        }

        // Make sure the collider is set to the target values at the end
        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y, targetCenterZ);
    }
    #endregion
}