using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Settings")]

    [Range(0.5f, 1)]
    [SerializeField] private float minCameraDistance = 1;

    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 1.5f;

    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 3;

    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector2 aimInput;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = Vector3.Lerp(aim.position, DesieredAimPosition(), Time.deltaTime * cameraSensetivity);
    }

    private Vector3 DesieredAimPosition()
    {

        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredAimPosition = GetMousePosition();
        Vector3 aimDirection = (desiredAimPosition - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, desiredAimPosition);
        float clampedDistance = Mathf.Clamp(distance, minCameraDistance, actualMaxCameraDistance);

        desiredAimPosition = transform.position + aimDirection * clampedDistance;
        desiredAimPosition.y = transform.position.y + 1;

        return desiredAimPosition;
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }
}
