using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController controller;
    private Animator animator;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private bool isRunning;
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityScale = 9.81f;

    private float verticalVelocity;

    [Header("Aim Settings")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector2 moveInput;
    private Vector2 aimInput;
    private Vector3 aimDirection;

    private void Start()
    {
        player = GetComponent<Player>();    

        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardMouse();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(moveDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(moveDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnimation = isRunning && moveDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
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
            aim.position = new Vector3(hitInfo.point.x, transform.position.y + 1, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (moveDirection.magnitude > 0)
        {
            controller.Move(moveDirection * Time.deltaTime * speed);
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

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Character.Movement.canceled += ctx => moveInput = Vector2.zero;

        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;

        controls.Character.Sprint.performed += ctx =>
        {
            speed = runSpeed;
            isRunning = true;

        };

        controls.Character.Sprint.canceled += ctx =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}
