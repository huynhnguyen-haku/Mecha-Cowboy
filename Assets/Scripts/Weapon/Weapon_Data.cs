using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "ScriptableObjects/Weapon Data", order = 1)]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;

    [Header("Weapon Details")]
    public ShotType shotType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int TotalReserveAmmo;

    [Header("Weapon Settings")]
    [Range(1, 6)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipSpeed = 1;
    [Range(4, 8)]
    public float laserDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;


    [Header("Spread")]
    public float minSpread;
    public float maxSpread;
    public float spreadIncreaseRate = 0.15f;

    [Header("Burst Fire")]
    public bool burstAvailable;
    public bool burstActive;
    public int burst_BulletsPerShot;
    public int burst_FireRate;
    public float burst_FireDelay = 0.1f;
}
