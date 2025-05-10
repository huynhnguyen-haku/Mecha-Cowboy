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
    //[SerializeField] private LayerMask preciseAim;
    [SerializeField] private LayerMask lockOnLayer;
    [SerializeField] private float minAimDistance = 1.5f;

    [Header("Lock-On Settings")]
    [SerializeField] private float lockOnRadius = 2f;
    [SerializeField] private float lockOnBreakDistance = 3f;
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

        if (isLockedOn && lockedEnemy != null)
        {
            RotatePlayerTowardsLockedTarget();
        }
    }

    public Transform Aim() => aim;

    private void ToggleLockOn(bool enable)
    {
        isLockedOn = enable;

        if (isLockedOn)
        {
            // Khi bật lock-on, tìm mục tiêu gần nhất
            CheckLockOn();
        }
        else
        {
            // Khi tắt lock-on, hủy mục tiêu đã khóa
            lockedEnemy = null;
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

        // Đảm bảo aimTarget hiển thị khi laser đang bật
        EnableAimTarget(true);
        if (aimTarget != null)
        {
            aimTarget.transform.position = aim.position; // Đồng bộ vị trí với aim
            aimTarget.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward); // Xoay sprite để đối diện camera
        }

        // Nếu đang lock-on, đảm bảo nòng súng hướng về mục tiêu lock-on
        if (isLockedOn && lockedEnemy != null)
        {
            weaponModel.transform.LookAt(lockedEnemy.position);
            weaponModel.gunPoint.LookAt(lockedEnemy.position);
        }
    }


    private void UpdateAimPosition()
    {
        if (isLockedOn && lockedEnemy != null)
        {
            // Khi lock-on, aim sẽ theo mục tiêu đã khóa
            aim.position = lockedEnemy.position;
            aimTarget.GetComponent<SpriteRenderer>().color = Color.red; // Đổi màu khi lock-on
        }
        else
        {
            // Khi không lock-on, aim sẽ theo vị trí chuột
            aim.position = GetMouseHitInfo().point;
            aimTarget.GetComponent<SpriteRenderer>().color = Color.white; // Màu mặc định
        }

        Vector3 directionToAim = aim.position - transform.position;
        float distanceToAim = directionToAim.magnitude;

        if (distanceToAim < minAimDistance)
        {
            directionToAim.Normalize();
            aim.position = transform.position + directionToAim * minAimDistance;
        }
    }

    private void CheckLockOn()
    {
        if (!isLockedOn)
        {
            // Nếu lock-on bị tắt, không làm gì cả
            return;
        }

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
            {
                lockedEnemy = closestEnemy;
            }
        }
    }

    private void RotatePlayerTowardsLockedTarget()
    {
        if (lockedEnemy == null || player == null)
            return;

        // Lấy hướng từ người chơi đến mục tiêu lock-on
        Vector3 directionToTarget = lockedEnemy.position - player.transform.position;
        directionToTarget.y = 0; // Loại bỏ trục Y để chỉ xoay trên mặt phẳng XZ

        if (directionToTarget.sqrMagnitude > 0.01f) // Kiểm tra nếu có hướng hợp lệ
        {
            // Tính toán góc quay
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Xoay người chơi dần dần về phía mục tiêu
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f // Tốc độ xoay (có thể điều chỉnh)
            );
        }
    }


    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity/*, preciseAim*/))
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

        controls.Character.ToggleLockOn.performed += ctx => ToggleLockOn(true);
        controls.Character.ToggleLockOn.canceled += ctx => ToggleLockOn(false);
    }
}
