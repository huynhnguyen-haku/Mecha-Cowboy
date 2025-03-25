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

[System.Serializable]

public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine;
    public int magazineCapacity;
    public int TotalReserveAmmo;

    [UnityEngine.Range(1, 3)]
    public float reloadSpeed =1;

    [UnityEngine.Range(1, 3)]
    public float equipSpeed = 1;

    [Space]
    public float fireRate = 1;
    private float lastShotTime;

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
       if (Time.time > lastShotTime + 1 / fireRate)
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
