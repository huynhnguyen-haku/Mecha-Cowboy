using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController controller;
    private Animator animator;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    public Vector2 moveInput { get; private set; }

    private float verticalVelocity;
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float gravityScale = 9.81f;

    private bool isRunning;

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
        ApplyRotation();
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

    private void ApplyRotation()
    {
        Vector3 aimDirection = player.aim.GetMousePosition() - transform.position;
        aimDirection.y = 0;
        aimDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
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
        if (!controller.isGrounded)
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
