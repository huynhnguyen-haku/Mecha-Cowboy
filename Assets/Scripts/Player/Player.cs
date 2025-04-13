using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; } 
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }
    public WeaponController weapon { get; private set; }
    public PlayerWeaponVisuals weaponVisuals { get; private set; }
    public PlayerInteraction interaction { get; private set; }
    public Player_Health health { get; private set; }
    public Ragdoll ragdoll { get; private set; } 
    public Animator anim { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<WeaponController>();
        weaponVisuals = GetComponentInChildren<PlayerWeaponVisuals>();
        interaction = GetComponent<PlayerInteraction>();
        health = GetComponent<Player_Health>();
        ragdoll = GetComponent<Ragdoll>();
        anim = GetComponentInChildren<Animator>();
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
