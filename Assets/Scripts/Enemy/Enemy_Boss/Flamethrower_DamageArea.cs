using System.Collections;
using UnityEngine;

public class Flamethrower_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;
    private CapsuleCollider capsuleCollider;

    private float damageCooldown;
    private float lastTimeDamage;
    private int flameDamage;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
        capsuleCollider = GetComponent<CapsuleCollider>();
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
            damagable.TakeDamage(flameDamage);
            lastTimeDamage = Time.time;
            damageCooldown = enemy.flameDamageCooldown;
        }

    }

    public void StartExpandingCollider()
    {
        StartCoroutine(ExpandCollider());
    }

    private IEnumerator ExpandCollider()
    {
        float duration = 1.5f; // Thời gian để mở rộng collider
        float elapsedTime = 0f;

        float startHeight = 1.2f;
        float targetHeight = 6f;

        float startCenterZ = -1.9f;
        float targetCenterZ = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp để thay đổi height và center.z
            capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y, Mathf.Lerp(startCenterZ, targetCenterZ, t));

            yield return null;
        }

        // Đảm bảo giá trị cuối cùng là chính xác
        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y, targetCenterZ);
    }
}

