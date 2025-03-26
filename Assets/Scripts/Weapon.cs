using System;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    Rifle,
    Shotgun,
    Sniper
}

public enum ShotType
{
    Single,
    Auto
}

[System.Serializable]

public class Weapon
{
    public WeaponType weaponType;

    [Header("Magazine Stats")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int TotalReserveAmmo;

    [Header("Weapon Stats")]
    public ShotType shotType;
    public float fireRate = 1;
    private float lastShotTime;

    [Space]

    [UnityEngine.Range(1, 3)]
    public float reloadSpeed =1;

    [UnityEngine.Range(1, 3)]
    public float equipSpeed = 1;

    [Header("Spread")]
    private float currenSpread;
    public float minSpread = 1;
    public float maxSpread = 3;

    public float spreadIncreaseRate = 0.15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1f;

    #region Spread Methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();
        float randomizedValue = UnityEngine.Random.Range(-currenSpread, currenSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }
    private void IncreaseSpread()
    {
        currenSpread = Mathf.Clamp(currenSpread + spreadIncreaseRate, minSpread, maxSpread);
    }
    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            DecreaseSpread();

        else
            IncreaseSpread();
        lastSpreadUpdateTime = Time.time;
    }
    private void DecreaseSpread()
    {
        currenSpread = Mathf.Clamp(currenSpread - (spreadIncreaseRate * 5), minSpread, maxSpread);
    }

    #endregion

    public bool CanShot()
    {
        if (HaveEnoughBullet() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }
        return false;
    }
    private bool ReadyToFire()
    {
        float timeBetweenShots = 60f / fireRate; // fireRate is now bullets per minute
        if (Time.time > lastShotTime + timeBetweenShots)
        {
            lastShotTime = Time.time;
            return true;
        }
        return false;
    }

    #region Reload Methods

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
        {
            return false;
        }

        if (TotalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    private bool HaveEnoughBullet() => bulletsInMagazine > 0;

    public void RefillBullets()
    {
        int bulletsToReload = magazineCapacity - bulletsInMagazine;
        if (bulletsToReload > TotalReserveAmmo)
        {
            bulletsToReload = TotalReserveAmmo;
        }

        TotalReserveAmmo -= bulletsToReload;
        bulletsInMagazine += bulletsToReload;

        if (TotalReserveAmmo < 0)
        {
            TotalReserveAmmo = 0;
        }
    }

    #endregion
}
