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
    public Enemy_Visual enemyVisual { get; private set; }

    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }
    #endregion

    [Header("Enemy Setting")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastDodgeTime;

    [Header("Attack Data")]
    public AttackData_EnemyMelee attackData;
    public List<AttackData_EnemyMelee> attackList;

    [Header("Special Attack")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastAxeThrowTime;
    public Transform axeStartPoint;

    //--------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        enemyVisual = GetComponent<Enemy_Visual>();

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
        enemyVisual.SetupVisual();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    private void InitializePerk()
    {
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            enemyVisual.SetupWeaponType(Enemy_MeleeWeaponType.Throw);
        }

        else if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            enemyVisual.SetupWeaponType(Enemy_MeleeWeaponType.OneHand);
        }

        else if (meleeType == EnemyMelee_Type.Dodge)
        {
            enemyVisual.SetupWeaponType(Enemy_MeleeWeaponType.Unarmed);
        }

        else if (meleeType == EnemyMelee_Type.Regular)
        {
            enemyVisual.SetupWeaponType(Enemy_MeleeWeaponType.OneHand);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        moveSpeed = moveSpeed * 0.5f;
        EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        Enemy_WeaponModel currentWeapon = enemyVisual.currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        if (currentWeapon != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    }

    public void EnableWeaponModel(bool active)
    {
        enemyVisual.currentWeaponModel.gameObject.SetActive(active);
    }

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

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoint <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
