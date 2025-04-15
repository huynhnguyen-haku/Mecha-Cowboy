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

    public int bulletDamage;

    public Weapon_Data weaponData { get; private set; }

    #region Magazine Details
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int TotalReserveAmmo;
    #endregion

    #region Weapon Details
    public ShotType shotType;
    public int bulletsPerShot { get; private set; }
    private float defaultFireRate;
    public float fireRate;
    private float lastShotTime;
    #endregion

    #region Weapon Settings
    public float reloadSpeed { get; private set; }
    public float equipSpeed { get; private set; }
    public float laserDistance { get; private set; }
    public float cameraDistance;
    #endregion

    #region Spread variables
    private float currenSpread;
    private float minSpread;
    private float maxSpread;

    private float spreadIncreaseRate;
    private float lastSpreadUpdateTime;
    private float spreadCooldown;
    #endregion

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

    #region Burst variables
    private bool burstAvailable;
    public bool burstActive;
    private int burst_BulletsPerShot;
    private int burst_FireRate;
    public float burst_FireDelay { get; private set; }
    #endregion

    #region Burst Fire Methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burst_FireDelay = 0;
            return true;
        }
        if (bulletsInMagazine < burst_BulletsPerShot)
        {
            return false;
        }

        return burstActive;
    }

    public void ToggleBurstMode()
    {
        if (burstAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burst_BulletsPerShot;
            fireRate = burst_FireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    #endregion


    public bool CanShot() => HaveEnoughBullet() && ReadyToFire();

    private bool ReadyToFire()
    {
        float timeBetweenShots = 60f / fireRate;
        if (Time.time > lastShotTime + timeBetweenShots)
        {
            lastShotTime = Time.time;
            return true;
        }
        return false;
    }

    public Weapon(Weapon_Data weapon_Data)
    {
        weaponType = weapon_Data.weaponType;

        bulletDamage = weapon_Data.bulletDamage;
        shotType = weapon_Data.shotType;
        bulletsPerShot = weapon_Data.bulletsPerShot;
        fireRate = weapon_Data.fireRate;

        bulletsInMagazine = weapon_Data.bulletsInMagazine;
        magazineCapacity = weapon_Data.magazineCapacity;
        TotalReserveAmmo = weapon_Data.TotalReserveAmmo;

        reloadSpeed = weapon_Data.reloadSpeed;
        equipSpeed = weapon_Data.equipSpeed;
        laserDistance = weapon_Data.laserDistance;
        cameraDistance = weapon_Data.cameraDistance;

        minSpread = weapon_Data.minSpread;
        maxSpread = weapon_Data.maxSpread;
        spreadIncreaseRate = weapon_Data.spreadIncreaseRate;

        burstAvailable = weapon_Data.burstAvailable;
        burstActive = weapon_Data.burstActive;
        burst_BulletsPerShot = weapon_Data.burst_BulletsPerShot;
        burst_FireRate = weapon_Data.burst_FireRate;
        burst_FireDelay = weapon_Data.burst_FireDelay;

        defaultFireRate = fireRate;
        this.weaponData = weapon_Data;
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
