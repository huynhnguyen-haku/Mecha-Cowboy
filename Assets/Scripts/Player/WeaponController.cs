using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the speed of the bullet from which our mass formula is derived

    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke("EquipStartingWeapon", 0.1f);
    }

    private void Update()
    {
        if(isShooting)
        {
            Shot();
        }
    }

    #region Slot Management - Equip, Pickup, Drop, Ready

    private void EquipStartingWeapon() => EquipWeapon(0); 

    private void EquipWeapon(int i)
    {
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }
    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
            return;

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModels();
    }
    private void DropWeapon()
    {
        if (HasOneWeapon())
        {
            return;
        }
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }
    public void SetWeaponReady(bool ready) => weaponReady = ready;
    public bool WeaponReady() => weaponReady;

    #endregion

    private void Shot()
    {
        if (WeaponReady() == false)
            return;

        if (currentWeapon.CanShot() == false)
            return;

        if (currentWeapon.shotType == ShotType.Single)
            isShooting = false;

        GameObject bullet = ObjectPool.instance.GetBullet();
        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();

        bullet.transform.position = GunPoint().position;
        bullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        rbBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbBullet.linearVelocity = BullectDirection() * bulletSpeed;

        player.weaponVisuals.PlayFireAnimation();
    }
    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }
    public Vector3 BullectDirection()
    {
        Transform aim = player.aim.Aim;

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public bool HasOneWeapon() => weaponSlots.Count <= 1;
    public Weapon CurrentWeapon() => currentWeapon;
    public Weapon BackupWeapon()
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
            {
                return weapon;
            }
        }
        return null;
    }

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;


        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }



    #endregion
}
