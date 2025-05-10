using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController controller;
    private Animator animator;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    public Vector2 moveInput { get; private set; }
    public bool isInCar;

    private float verticalVelocity;
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float gravityScale = 9.81f;

    private bool isRunning;

    private AudioSource walkSFX;
    private AudioSource runSFX;
    private bool canPlayFootstepsSFX;

    private void Start()
    {
        player = GetComponent<Player>();

        walkSFX = player.sfx.walkSFX;
        runSFX = player.sfx.runSFX;
        Invoke(nameof(EnableFootstepsSFX), 1f);

        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        if (player.health.playerIsDead)
            return;

        if (isInCar)
            return;

        if (!controller.enabled)
            return;

        ApplyMovement();
        ApplyRotation();
        AnimatorControllers();
    }

    public void SetPaused(bool isPaused)
    {
        // Nếu người chơi đang ở trong xe, không thay đổi trạng thái CharacterController
        if (isInCar)
        {
            Debug.Log("Player is in car, skipping CharacterController state change.");
            return;
        }

        // Kiểm tra nếu CharacterController hợp lệ trước khi thay đổi trạng thái
        if (controller != null && controller.enabled != !isPaused)
        {
            controller.enabled = !isPaused;

            // Dừng hoạt ảnh nếu cần
            if (animator != null)
            {
                animator.speed = isPaused ? 0 : 1;
            }
        }
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
        // Bỏ qua xoay nếu đang lock-on
        if (player.aim.isLockedOn && player.aim.lockedEnemy != null)
            return;

        Vector3 aimDirection = player.aim.GetMouseHitInfo().point - transform.position;
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
            PlayFootstepsSFX();
            controller.Move(moveDirection * Time.deltaTime * speed);
        }
    }

    private void EnableFootstepsSFX() => canPlayFootstepsSFX = true;

    private void PlayFootstepsSFX()
    {
        if (!canPlayFootstepsSFX)
            return;

        if (isRunning)
        {
            if (runSFX.isPlaying == false)
                runSFX.Play();
        }
        else
        {
            if (walkSFX.isPlaying == false)
                walkSFX.Play();
        }
    }

    private void StopFootstepsSFX()
    {
        walkSFX.Stop();
        runSFX.Stop();
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
        controls.Character.Movement.canceled += ctx =>
        {
            StopFootstepsSFX();
            moveInput = Vector2.zero;
        };

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
