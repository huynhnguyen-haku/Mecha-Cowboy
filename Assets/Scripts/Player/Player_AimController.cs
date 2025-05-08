using System;
using UnityEngine;

public class Player_AimController : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;
    private CameraManager cameraManager;

    [Header("Aim Visual - Laser")]
    public LineRenderer aimLaser; // Change here to be accessible from other scripts
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
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private float offsetChangeRate = 6;
    private float offsetY;

    [Header("Aim Layers")]
    [SerializeField] private LayerMask preciseAim;
    [SerializeField] private LayerMask regularAim;

    [Header("Aim Distance Constraints")]
    [SerializeField] private float minAimDistance = 1.5f; // Khoảng cách tối thiểu giữa aimTarget và người chơi


    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();
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

    public void EnableAimTarget(bool enable) => aimTarget.SetActive(enable);

    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    private void UpdateAimVisual()
    {
        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        aimLaser.enabled = player.weapon.WeaponReady(); // Enable laser only when weapon is ready
        if (aimLaser.enabled == false)
            return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);


        Transform gunPoint = player.weapon.GunPoint();

        float laserTipLength = 0.5f;
        float gunDistance = player.weapon.CurrentWeapon().laserDistance;

        Vector3 laserDirection = player.weapon.BullectDirection();
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        // Perform a raycast to detect obstacles
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, gunDistance))
        {
            endPoint = hitInfo.point;
            laserTipLength = 0;
        }

        // Update the laser positions
        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    private void UpdateAimPosition()
    {
        aim.position = GetMouseHitInfo().point;

        Vector3 newAimPosition = isAimingPrecisly ? aim.position : transform.position;

        // Giới hạn khoảng cách tối thiểu giữa aimTarget và người chơi
        Vector3 directionToAim = aim.position - transform.position;
        float distanceToAim = directionToAim.magnitude;

        if (distanceToAim < minAimDistance)
        {
            // Đặt aimTarget ở khoảng cách tối thiểu
            directionToAim.Normalize();
            aim.position = transform.position + directionToAim * minAimDistance;
        }

        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);
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
