using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the speed of the bullet from which our mass formula is derived

    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    #region Slot Management - Equip, Pickup, Drop

    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.SwitchOffWeaponModels();
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
            return;

        weaponSlots.Add(newWeapon);
    }

    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
        {
            return;
        }
        weaponSlots.Remove(currentWeapon);
        currentWeapon = weaponSlots[0];
    }

    #endregion

    private void Shot()
    {
        if (currentWeapon.CanShot() == false)
        {
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();

        rbBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbBullet.linearVelocity = BullectDirection() * bulletSpeed;

        Destroy(bullet, 10); // This is a bullet life time, don't mistake it with destroy when colliding in Bullet.cs
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BullectDirection()
    {
        Transform aim = player.aim.Aim;

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;
        
        //weaponHolder.LookAt(aim);
        //gunPoint.LookAt(aim);

        return direction;
    }

    public Transform GunPoint => gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;
        controls.Character.Fire.performed += context => Shot();
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload())
            {
                //currentWeapon.bulletsInMagazine = currentWeapon.TotalReserveAmmo;
                player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }

    #endregion
}
