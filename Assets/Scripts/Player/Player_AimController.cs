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

    [Header("Aim Controls")]
    [SerializeField] private float preciseAimCameraDistance = 6;
    [SerializeField] private float regularAimCameraDistance = 8;
    [SerializeField] private float cameraChangeRate = 5;

    [Header("Aim Offset")]
    public bool isAimingPrecisly;
    private float offsetY;
    [SerializeField] private Transform aim;
    [SerializeField] private float offsetChangeRate = 6;

    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim;
    [SerializeField] private LayerMask regularAim;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Aim Distance Constraints")]
    [SerializeField] private float minAimDistance = 1.5f;

    [Header("Lock-On Settings")]
    [SerializeField] private float lockOnRadius = 2f;
    [SerializeField] private float lockOnBreakDistance = 3f;
    private Transform lockedEnemy;
    private bool isLockedOn;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        instance = this;

        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();

        // Đảm bảo aimTarget ban đầu được tắt nếu không cần thiết
        if (aimTarget != null)
        {
            aimTarget.SetActive(false);
        }
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
    }

    private void TogglePreciseAim(bool enable)
    {
        isAimingPrecisly = !isAimingPrecisly;
        Cursor.visible = false;

        if (enable)
        {
            cameraManager.ChangeCameraDistance(preciseAimCameraDistance, cameraChangeRate);
            Time.timeScale = 0.9f;
        }
        else
        {
            cameraManager.ChangeCameraDistance(regularAimCameraDistance, cameraChangeRate);
            Time.timeScale = 1f;
        }
    }

    public void EnableLaserAim(bool enable) => aimLaser.enabled = enable;

    public void EnableAimTarget(bool enable)
    {
        if (aimTarget != null)
        {
            aimTarget.SetActive(enable);
        }
    }

    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    private void UpdateAimVisual()
    {
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        aimLaser.enabled = player.weapon.WeaponReady();
        if (aimLaser.enabled == false)
        {
            EnableAimTarget(false); // Tắt aimTarget nếu không có laser
            return;
        }

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weapon.GunPoint();

        float laserTipLength = 0.5f;
        float gunDistance = player.weapon.CurrentWeapon().laserDistance;

        Vector3 laserDirection = player.weapon.BullectDirection();
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, gunDistance))
        {
            endPoint = hitInfo.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);

        // Đảm bảo aimTarget hiển thị khi laser đang bật
        EnableAimTarget(true);
        if (aimTarget != null)
        {
            aimTarget.transform.position = aim.position; // Đồng bộ vị trí với aim
            aimTarget.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward); // Xoay sprite để đối diện camera
        }
    }

    private void UpdateAimPosition()
    {
        if (isLockedOn && lockedEnemy != null)
        {
            aim.position = lockedEnemy.position;
        }
        else
        {
            aim.position = GetMouseHitInfo().point;
        }

        CheckLockOn();

        Vector3 newAimPosition = isAimingPrecisly ? aim.position : transform.position;

        Vector3 directionToAim = aim.position - transform.position;
        float distanceToAim = directionToAim.magnitude;

        if (distanceToAim < minAimDistance)
        {
            directionToAim.Normalize();
            aim.position = transform.position + directionToAim * minAimDistance;
        }

        if (isLockedOn)
        {
            aimTarget.GetComponent<SpriteRenderer>().color = Color.red; // Đổi màu khi lock-on
        }
        else
        {
            aimTarget.GetComponent<SpriteRenderer>().color = Color.white; // Màu mặc định
        }

        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
    }

    private void CheckLockOn()
    {
        if (isLockedOn && lockedEnemy != null)
        {
            Vector3 mousePos = GetMouseHitInfo().point;
            float distanceToEnemy = Vector3.Distance(mousePos, lockedEnemy.position);

            if (distanceToEnemy > lockOnBreakDistance)
            {
                isLockedOn = false;
                lockedEnemy = null;
            }
            return;
        }

        Collider[] enemies = Physics.OverlapSphere(aim.position, lockOnRadius, enemyLayer);
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
            {
                lockedEnemy = closestEnemy;
                isLockedOn = true;
            }
        }
    }

    private float AdjustedOffsetY()
    {
        if (isAimingPrecisly)
            offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChangeRate);
        else
            offsetY = Mathf.Lerp(offsetY, 1, Time.deltaTime * offsetChangeRate);

        return offsetY;
    }

    public Transform Aim() => aim;

    public bool CanAimPrecisly() => isAimingPrecisly;

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, preciseAim))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
        return lastKnownMouseHit;
    }

    #region Camera Region

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

    private void UpdateCameraPosition()
    {
        bool canMoveCamera = Vector3.Distance(cameraTarget.position, DesieredCameraPosition()) > 1f;
        if (!canMoveCamera)
            return;

        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), Time.deltaTime * cameraSensetivity);
    }

    #endregion

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => mouseInput = Vector2.zero;

        controls.Character.TogglePreciseAim.performed += ctx => TogglePreciseAim(true);
        controls.Character.TogglePreciseAim.canceled += ctx => TogglePreciseAim(false);
    }
}