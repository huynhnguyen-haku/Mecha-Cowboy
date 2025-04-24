using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; } 
    public Player_AimController aim { get; private set; }
    public Player_Movement movement { get; private set; }
    public Player_WeaponController weapon { get; private set; }
    public Player_WeaponVisuals weaponVisuals { get; private set; }
    public Player_Interaction interaction { get; private set; }
    public Player_Health health { get; private set; }
    public Ragdoll ragdoll { get; private set; } 
    public Animator anim { get; private set; }
    public bool controlsEnabled { get; private set; } 

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<Player_AimController>();
        movement = GetComponent<Player_Movement>();
        weapon = GetComponent<Player_WeaponController>();
        weaponVisuals = GetComponentInChildren<Player_WeaponVisuals>();
        interaction = GetComponent<Player_Interaction>();
        health = GetComponent<Player_Health>();
        ragdoll = GetComponent<Ragdoll>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Character.ToggleMissionUI.performed += ctx => UI.instance.inGameUI.ToggleMissionUI();
        controls.Character.TogglePauseUI.performed += ctx => UI.instance.TogglePauseUI();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }
}
