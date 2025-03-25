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

    public bool CanShot()
    {
        return HaveEnoughBullet();
    }
    private bool HaveEnoughBullet()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }
        return false;
    }
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
}
