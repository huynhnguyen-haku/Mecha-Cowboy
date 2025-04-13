using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData_EnemyMelee
{
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
    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }
    #endregion

    #region Fields
    [Header("Enemy Setting")]
    public EnemyMelee_Type meleeType;
    public Enemy_MeleeWeaponType weaponType;

    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastDodgeTime;

    [Header("Attack Data")]
    public AttackData_EnemyMelee attackData;
    public List<AttackData_EnemyMelee> attackList;
    private Enemy_WeaponModel currentWeapon;
    private bool isAttackReady;

    [Header("Special Attack")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastAxeThrowTime;
    public Transform axeStartPoint;
    [Space]
    [SerializeField] private GameObject meleeAttackFX;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // We use ragdoll instead of animation
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        lastDodgeTime = Time.realtimeSinceStartup;
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        InitializePerk();
        visual.SetupVisual();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        AttackCheck();
    }

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
        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        walkSpeed = walkSpeed * 0.5f;
        visual.EnableWeaponModel(false);
    }

    public override void Die()
    {
        base.Die();
        if (stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }
    #endregion

    #region Attack Methods

    public void AttackCheck()
    {
        if (isAttackReady == false)
            return;

        foreach (Transform attackPoint in currentWeapon.damagePoints)
        {
            Collider[] detectedHits = 
                Physics.OverlapSphere(attackPoint.position, currentWeapon.attackRadius, whatIsPlayer);

            for (int i = 0; i < detectedHits.Length; i++)
            {
                I_Damagable damagable = detectedHits[i].GetComponent<I_Damagable>();

                if (damagable != null)
                {
                    damagable.TakeDamage();
                    isAttackReady = false;
                    GameObject newAttackFX = ObjectPool.instance.GetObject(meleeAttackFX, attackPoint);

                    ObjectPool.instance.ReturnObject(newAttackFX, 1);
                    return;
                }
            }
        }
    }

    public void EnableAttackCheck(bool enable)
    {
        isAttackReady = enable;
    }

    public void UpdateAttackData()
    {
        currentWeapon = visual.currentWeaponModel.GetComponent<Enemy_WeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    public void ThrowAxe()
    {
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);
        newAxe.GetComponent<Enemy_Axe>().AxeSetup(axeFlySpeed, player, axeAimTimer);
    }

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

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    }

    #endregion

    #region Ability Methods
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
    protected override void InitializePerk()
    {
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponType = Enemy_MeleeWeaponType.Throw;
        }
        else if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }
        else if (meleeType == EnemyMelee_Type.Dodge)
        {
            weaponType = Enemy_MeleeWeaponType.Unarmed;
        }
        else if (meleeType == EnemyMelee_Type.Regular)
        {
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }
    }
    #endregion
}
