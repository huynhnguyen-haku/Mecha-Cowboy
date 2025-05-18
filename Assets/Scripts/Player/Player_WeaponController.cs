using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly;
    [Space]
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the speed of the bullet from which our mass formula is derived

    [SerializeField] private List<Weapon_Data> defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet Settings")]
    [SerializeField] private float bulletImpactForce;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 3; // Max weapon slots

    [SerializeField] private GameObject weaponPickupPrefab;

    [Header("Minigun")]
    private bool isSpinning;
    private bool isFireSFXPlaying;
    private Coroutine spinCoroutine;
    private Coroutine spinnerCoroutine; // Coroutine để quản lý việc xoay spinner


    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (isShooting)
        {
            Shot();
        }
    }

    public void UpdateWeaponUI()
    {
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }

    #region Slot Management - Equip, Pickup, Drop, Ready
    public void SetDefaultWeapon(List<Weapon_Data> newWeaponData)
    {
        defaultWeaponData = new List<Weapon_Data>(newWeaponData);
        weaponSlots.Clear();

        foreach (Weapon_Data weaponData in defaultWeaponData)
        {
            PickupWeapon(new Weapon(weaponData));
        }

        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
            return;

        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
        //CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
        UpdateWeaponUI();
    }

    public void PickupWeapon(Weapon newWeapon)
    {

        // If the weapon is already in the inventory, add the ammo to the existing weapon
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).TotalReserveAmmo += newWeapon.TotalReserveAmmo;
            UpdateWeaponUI();
            return;
        }

        // If the weapon is not in the inventory, add it to the inventory
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModels();
        UpdateWeaponUI();
    }

    private void DropWeapon()
    {
        if (HasOneWeapon())
        {
            return;
        }
        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Weapon_PickUp>().SetUpPickupWeapon(currentWeapon, player.transform);
    }

    public void SetWeaponReady(bool ready)
    {
        weaponReady = ready;
    }

    public bool WeaponReady() => weaponReady;

    #endregion

    #region Shooting Mechanics
    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burst_FireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shot()
    {
        if (player.health.playerIsDead)
            return;

        if (WeaponReady() == false)
            return;

        if (currentWeapon.CanShot() == false)
            return;

        if (currentWeapon.weaponType == WeaponType.Minigun)
        {
            if (!isSpinning)
            {
                // Start spinning up the minigun
                spinCoroutine = StartCoroutine(SpinUpMinigun());
            }
            return;
        }

        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shotType == ShotType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            TriggerEnemyDodge();
            return;
        }
        FireSingleBullet();
        TriggerEnemyDodge();
    }

    private IEnumerator SpinUpMinigun()
    {
        isSpinning = true;

        // Play start spin sound
        var spinSFX = player.weaponVisuals.CurrentWeaponModel().spinSFX;
        if (spinSFX != null && !spinSFX.isPlaying)
            spinSFX.Play();

        // Spin up the minigun
        var spinner = player.weaponVisuals.CurrentWeaponModel().minigunSpinner;
        if (spinner != null)
            spinnerCoroutine = StartCoroutine(SpinMinigunSpinner());

        yield return new WaitForSeconds(1.25f);

        if (!isFireSFXPlaying)
        {
            var fireSFX = player.weaponVisuals.CurrentWeaponModel().fireSFX;
            fireSFX.Play();
            isFireSFXPlaying = true;
        }

        while (isShooting && currentWeapon.weaponType == WeaponType.Minigun)
        {
            if (!currentWeapon.HaveEnoughBullet())
            {
                StopMinigunFire();
                yield break;
            }
            FireSingleBullet();
            yield return new WaitForSeconds(60f / currentWeapon.fireRate);
        }
        StopMinigunFire();
    }

    private IEnumerator SpinMinigunSpinner()
    {
        var spinner = player.weaponVisuals.CurrentWeaponModel().minigunSpinner;

        while (isSpinning)
        {
            if (spinner != null)
                spinner.Rotate(0, 0, -1000f * Time.deltaTime);

            yield return null;
        }
    }

    private void StopMinigunFire()
    {
        isSpinning = false;

        // Stop spinner and fireSFX
        if (spinnerCoroutine != null)
        {
            StopCoroutine(spinnerCoroutine);
            spinnerCoroutine = null;
        }

        if (isFireSFXPlaying)
        {
            var fireSFX = player.weaponVisuals.CurrentWeaponModel().fireSFX;
            fireSFX.Stop();
            isFireSFXPlaying = false;
        }

        // Play overheat sound
        var endSpinSFX = player.weaponVisuals.CurrentWeaponModel().endSpinSFX;
        if (endSpinSFX != null && !endSpinSFX.isPlaying)
            endSpinSFX.Play();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        UpdateWeaponUI();

        if (WeaponType.Shotgun == currentWeapon.weaponType)
            player.weaponVisuals.CurrentWeaponModel().fireSFX.Play();

        else if (WeaponType.Minigun != currentWeapon.weaponType)
            player.weaponVisuals.CurrentWeaponModel().fireSFX.PlayOneShot(player.weaponVisuals.CurrentWeaponModel().fireSFX.clip);

        GameObject bullet = ObjectPool.instance.GetObject(bulletPrefab, GunPoint());
        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();

        bullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(whatIsAlly, currentWeapon.bulletDamage, bulletImpactForce);

        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbBullet.linearVelocity = bulletsDirection * bulletSpeed;
    }


    private void Reload() // This only used for playing reload animation (the real one is in Animation event)
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
        player.weaponVisuals.CurrentWeaponModel().reloadSFX.Play();
    }
    #endregion

    #region Utility Methods
    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        return direction;
    }


    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public bool HasOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }
    public Weapon CurrentWeapon() => currentWeapon;
    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy_melee = hit.collider.GetComponentInParent<Enemy_Melee>();
            if (enemy_melee != null)
            {
                enemy_melee.ActivateDodgeRoll();
            }
        }
    }
    #endregion

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;


        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.ToggleBurstMode.performed += context => currentWeapon.ToggleBurstMode();
        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
        controls.Character.Fire.canceled += context =>
        {
            isShooting = false;

            if (currentWeapon.weaponType == WeaponType.Minigun)
            {
                if (isSpinning)
                {
                    StopCoroutine(spinCoroutine);
                    isSpinning = false;
                    var spinSFX = player.weaponVisuals.CurrentWeaponModel().spinSFX;
                    spinSFX.Stop();

                    var endSpinSFX = player.weaponVisuals.CurrentWeaponModel().endSpinSFX;
                    endSpinSFX.Play();
                }

                if (isFireSFXPlaying)
                {
                    isFireSFXPlaying = false;
                    var fireSFX = player.weaponVisuals.CurrentWeaponModel().fireSFX;
                    fireSFX.Stop();
                }
            }
        };
    }

    #endregion
}
