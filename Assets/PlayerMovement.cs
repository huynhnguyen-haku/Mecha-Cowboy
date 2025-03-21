using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController controller;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravityScale = 9.81f;

    private float verticalVelocity;

    [Header("Aim Settings")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector2 moveInput;
    private Vector2 aimInput;
    private Vector3 aimDirection;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Character.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Character.Movement.canceled += ctx => moveInput = Vector2.zero;

        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardMouse();

    }

    private void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            aimDirection = hitInfo.point - transform.position;
            aimDirection.y = 0;
            aimDirection.Normalize();

            transform.forward = aimDirection;
            aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (moveDirection.magnitude > 0)
        {
            controller.Move(moveDirection * Time.deltaTime * moveSpeed);
        }
    }

    private void ApplyGravity()
    {
        if(!controller.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            moveDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -0.5f;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
