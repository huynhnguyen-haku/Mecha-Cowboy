using System;
using UnityEngine;

public class Player_AimController : MonoBehaviour
{
    public static Player_AimController instance;

    private Player player;
    private PlayerControls controls;
    private CameraManager cameraManager;

    [Header("Aim Visual - Laser")]
    public LineRenderer aimLaser;
    public GameObject aimTarget;

    [Header("Camera Controls")]
    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5;

    [Header("Aim Settings")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayer;
    [SerializeField] private LayerMask lockOnLayer;

    [Header("Lock-On Settings")]
    [SerializeField] private float lockOnRadius = 2f;
    public Transform lockedEnemy;
    public bool isLockedOn;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        instance = this;
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();

        if (aimTarget != null)
            aimTarget.SetActive(false);
    }

    private void Update()
    {
        if (player.health.playerIsDead)
            return;

        if (!player.controlsEnabled)
            return;

        UpdateAimVisual();
        UpdateAimPosition();
        UpdateCameraPosition();

        if (isLockedOn && lockedEnemy != null)
            RotatePlayerTowardsLockedTarget();
    }

    public Transform Aim() => aim;

    #region Aim Logic

    // Update aim laser and weapon visuals
    private void UpdateAimVisual()
    {
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        aimLaser.enabled = player.weapon.WeaponReady();
        if (!aimLaser.enabled)
        {
            EnableAimTarget(false);
            return;
        }

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weapon.GunPoint();

        float laserTipLength = 0.5f;
        float gunDistance = player.weapon.CurrentWeapon().laserDistance;

        Vector3 laserDirection = player.weapon.BulletDirection();
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, gunDistance))
        {
            endPoint = hitInfo.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);

        // Show aimTarget when laser is enabled
        EnableAimTarget(true);
        if (aimTarget != null)
        {
            aimTarget.transform.position = aim.position;
            aimTarget.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        // If locked on, force weapon to look at target
        if (isLockedOn && lockedEnemy != null)
        {
            weaponModel.transform.LookAt(lockedEnemy.position);
            weaponModel.gunPoint.LookAt(lockedEnemy.position);
        }
    }

    // Update aim position based on lock-on or mouse
    private void UpdateAimPosition()
    {
        if (isLockedOn && lockedEnemy != null)
        {
            aim.position = lockedEnemy.position;
            aimTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            aim.position = GetMouseHitInfo().point;
            aimTarget.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void EnableLaserAim(bool enable) => aimLaser.enabled = enable;

    public void EnableAimTarget(bool enable)
    {
        if (aimTarget != null)
            aimTarget.SetActive(enable);
    }

    // Get the world position where the mouse is aiming
    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayer))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
        return lastKnownMouseHit;
    }

    // Get camera target for aiming
    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    #endregion

    #region Lock-On Logic

    // Enable or disable lock-on and update target
    private void ToggleLockOn(bool enable)
    {
        isLockedOn = enable;

        if (isLockedOn)
            CheckLockOn(); // Find nearest target
        else
            lockedEnemy = null; // Clear target
    }

    // Find and set the closest enemy for lock-on
    private void CheckLockOn()
    {
        if (!isLockedOn)
            return;

        Collider[] enemies = Physics.OverlapSphere(aim.position, lockOnRadius, lockOnLayer);
        if (enemies.Length > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;
            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(aim.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            if (closestEnemy != null)
                lockedEnemy = closestEnemy;
        }
    }

    // Smoothly rotate player toward locked target
    private void RotatePlayerTowardsLockedTarget()
    {
        if (lockedEnemy == null || player == null)
            return;

        Vector3 directionToTarget = lockedEnemy.position - player.transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
    }

    #endregion

    #region Camera Logic

    // Calculate desired camera position based on aim and movement
    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampedDistance = Mathf.Clamp(distance, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    // Smoothly update camera position toward desired position
    private void UpdateCameraPosition()
    {
        bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesieredCameraPosition()) > 1f;
        if (!canMoveCamera)
            return;

        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), Time.deltaTime * cameraSensetivity);
    }

    #endregion

    #region Input Events

    // Assign input events for aiming and lock-on
    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => mouseInput = Vector2.zero;

        controls.Character.ToggleLockOn.performed += ctx => ToggleLockOn(true);
        controls.Character.ToggleLockOn.canceled += ctx => ToggleLockOn(false);
    }

    #endregion
}
