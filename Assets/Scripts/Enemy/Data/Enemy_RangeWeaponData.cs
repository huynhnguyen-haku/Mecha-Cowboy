using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Range Weapon Data", order = 1)]

public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon Details")]
    public Enemy_RangeWeaponType weaponType;
    public float fireRate = 120;

    public int minBulletPerAttack = 1;
    public int maxBulletPerAttack = 1;

    public int minWeaponCooldown = 2;
    public int maxWeaponCooldown = 3;

    [Header("Bullet Details")]
    public float bulletSpeed = 20;
    public float weaponSpread = 0.1f;

    public int GetRandomBulletPerAttack()
    {
        return Random.Range(minBulletPerAttack, maxBulletPerAttack);
    }
    public float GetRandomWeaponCooldown()
    {
        return Random.Range(minWeaponCooldown, maxWeaponCooldown);
    }

    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = UnityEngine.Random.Range(-weaponSpread, weaponSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }
}
