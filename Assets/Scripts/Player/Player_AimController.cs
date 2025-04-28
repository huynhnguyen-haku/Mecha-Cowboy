using System;
using UnityEngine;

public class Player_AimController : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    public LineRenderer aimLaser; // Change here to be accessible from other scripts

    [Header("Camera Controls")]

    [SerializeField] private Transform cameraTarget;
    [Range(0.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5;

    [Header("Aim Controls")]

    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockedOnTarget;

    [Space]

    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 mouseInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (player.health.playerIsDead)
            return;

        if (!player.controlsEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.P))
            isAimingPrecisly = !isAimingPrecisly; // Toggle aiming mode, switch between aiming normaly and precisly

        if (Input.GetKeyDown(KeyCode.L))
            isLockedOnTarget = !isLockedOnTarget; // Toggle lock on target mode

        UpdateAimVisual();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    public void EnableLaserAim(bool enable) => aimLaser.enabled = enable;

    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    private void UpdateAimVisual()
    {
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
        Transform target = Target();

        if (target != null && isLockedOnTarget)
        {
            if (target.GetComponent<Renderer>() != null)
                aim.position = target.GetComponent<Renderer>().bounds.center;

            else
                aim.position = target.position;

            return;
        }

        aim.position = GetMouseHitInfo().point;
        if (!isAimingPrecisly)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }


    public Transform Target()
    {
        Transform target = null;
        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }
        return target;
    }

    public Transform Aim => aim;
    public bool CanAimPrecisly() => isAimingPrecisly;

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
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
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), Time.deltaTime * cameraSensetivity);
    }


    #endregion

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => mouseInput = Vector2.zero;
    }
}
