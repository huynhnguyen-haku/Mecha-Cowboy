using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData_EnemyMelee
{
    public int attackDamage;
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class Enemy_Melee : Enemy
{
    public Enemy_MeleeSFX meleeSFX { get; private set; } // Reference to the sound effects component

    #region States
    public IdleState_Melee idleState { get; private set; } // Reference to idle state
    public MoveState_Melee moveState { get; private set; } // Reference to move state
    public RecoveryState_Melee recoveryState { get; private set; } // Reference to recovery state
    public ChaseState_Melee chaseState { get; private set; } // Reference to chase state
    public AttackState_Melee attackState { get; private set; } // Reference to attack state
    public DeadState_Melee deadState { get; private set; } // Reference to dead state
    public AbilityState_Melee abilityState { get; private set; } // Reference to ability state
    #endregion

    #region Fields
    [Header("Enemy Setting")]
    public EnemyMelee_Type meleeType; // Type of melee enemy
    public Enemy_MeleeWeaponType weaponType; // Type of weapon used by the enemy

    [Header("Shield")]
    public int shieldDurability; // Durability of the shield
    public Transform shieldTransform; // Transform of the shield object

    [Header("Dodge")]
    public float dodgeCooldown; // Cooldown time for dodge roll
    private float lastDodgeTime; // Last time a dodge was performed

    [Header("Attack Data")]
    public AttackData_EnemyMelee attackData; // Current attack data
    public List<AttackData_EnemyMelee> attackList; // List of possible attacks
    private Enemy_WeaponModel currentWeapon; // Current weapon model
    private bool isAttackReady; // Tracks if an attack is ready

    [Header("Special Attack")]
    public int axeDamage; // Damage dealt by axe throw
    public GameObject axePrefab; // Prefab for the axe
    public float axeFlySpeed; // Speed of the thrown axe
    public float axeAimTimer; // Time to aim before throwing the axe
    public float axeThrowCooldown; // Cooldown for axe throw
    private float lastAxeThrowTime; // Last time an axe was thrown
    public Transform axeStartPoint; // Starting point for axe throw

    [Header("Minimap Icon")]
    private GameObject minimapIcon; // Reference to the minimap icon

    [Space]

    [SerializeField] private GameObject meleeAttackFX; // Visual effect for melee attacks
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        meleeSFX = GetComponent<Enemy_MeleeSFX>();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // We use ragdoll instead of animation
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        // Add minimap icon
        if (minimapIcon == null)
        {
            var minimapSprite = GetComponentInChildren<MinimapSprite>(true);
            if (minimapSprite != null)
                minimapIcon = minimapSprite.gameObject;
        }

        lastDodgeTime = Time.realtimeSinceStartup;
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        InitializePerk();
        visual.SetupVisual();

        RandomizeFirstAttack();
        UpdateAttackData();
    }

    // Randomize the first attack to avoid repetition
    private void RandomizeFirstAttack()
    {
        // This method is used to fix the bug where the enemy always uses the same attack
        if (attackList.Count > 0)
            attackData = attackList[Random.Range(0, attackList.Count)];
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        MeleeAttackCheck(currentWeapon.damagePoints, currentWeapon.attackCheckRadius, meleeAttackFX, attackData.attackDamage);
    }

    // Draw gizmos in the editor for debugging
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
    #endregion

    #region State Methods
    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        UpdateAttackData(); // Update attack data when entering battle mode
        stateMachine.ChangeState(recoveryState);
    }

    // Trigger the ability and adjust movement
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        walkSpeed = walkSpeed * 0.5f;
        visual.EnableWeaponModel(false);
    }

    // Handle the death of the enemy
    public override void Die()
    {
        base.Die();

        if (!HealthController.muteDeathSound) // Check if the death sound should be muted
            meleeSFX.deadSFX.Play();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);

        if (minimapIcon != null)
            minimapIcon.SetActive(false);

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Enemy"));
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }

    // Update the current weapon and attack data
    public void UpdateAttackData()
    {
        currentWeapon = visual.currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    // Throw an axe at the player
    public void ThrowAxe()
    {
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);
        newAxe.GetComponent<Enemy_Axe>().AxeSetup(axeFlySpeed, player, axeAimTimer, axeDamage);
    }

    // Check if the enemy can throw an axe
    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastAxeThrowTime + axeThrowCooldown)
        {
            lastAxeThrowTime = Time.time;
            return true;
        }
        return false;
    }

    // Check if the player is within attack range
    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    }
    #endregion

    #region Ability Methods
    // Activate a dodge roll if conditions are met
    public void ActivateDodgeRoll()
    {
        // Only dodge roll if the player is outside of attack range
        if (Vector3.Distance(transform.position, player.position) < attackData.attackRange)
            return;

        // Only dodge roll if the enemy is a dodge type
        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        // Only dodge roll during chase state
        if (stateMachine.currentState != chaseState)
            return;

        if (Time.time > lastDodgeTime + dodgeCooldown)
        {
            lastDodgeTime = Time.time;
            anim.SetTrigger("Dodge");
        }
    }
    #endregion

    #region Initialization Methods
    // Initialize perks based on enemy type
    protected override void InitializePerk()
    {
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }

        else if (meleeType == EnemyMelee_Type.Dodge)
            weaponType = Enemy_MeleeWeaponType.Unarmed;

        else if (meleeType == EnemyMelee_Type.AxeThrow)
            weaponType = Enemy_MeleeWeaponType.Throw;
    }
    #endregion
}