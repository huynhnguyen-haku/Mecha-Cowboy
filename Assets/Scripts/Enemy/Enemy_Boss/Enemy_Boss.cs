using UnityEngine;

public enum BossWeaponType { Fist, Hammer}
public class Enemy_Boss : Enemy
{

    [Header("Boss Detail")]
    public BossWeaponType weaponType;
    public float attackRange;
    public float actionCooldown = 10;
    public int attackAnimationCount;

    [Header("Jump Attack")]
    public float impactRadius = 2.5f;
    public float impactPower = 10;
    [SerializeField] private float upwardsMulti = 10;
    [Space]
    public float travelTimeToTarget = 1;
    public float jumpAttackCooldown = 10;
    private float lastTimeJump;
    public float minJumpDistanceRequired;
    public Transform impactPoint;

    [Header("Abilities")]
    public float flamethrowDuration;
    public float abilityCooldown;   
    private float lastTimeAbility;  
    public ParticleSystem flamethrower;
    public bool flamethrowerActive { get; private set; }


    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public Enemy_BossVisual bossVisual { get; private set; }
    public DeadState_Boss deadState { get; private set; }

    [Space]

    [SerializeField] private LayerMask whatToIgnore;


    protected override void Awake()
    {
        base.Awake();
        bossVisual = GetComponent<Enemy_BossVisual>();
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
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
    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public void ActivateFlamethrower(bool activate)
    {
        flamethrowerActive = activate;

        if (!activate)
        {
            flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        var mainModule = flamethrower.main;
        var extraModule = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        mainModule.duration = flamethrowDuration;
        extraModule.duration = flamethrowDuration;

        flamethrower.Clear();
        flamethrower.Play();
    }

    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time > lastTimeJump + jumpAttackCooldown && IsPlayerInClearSight())
        {
            return true;
        }
        return false;
    }

    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint;

        if (impactPoint == null)
            impactPoint = transform;

        Collider[] colliders = Physics.OverlapSphere(impactPoint.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMulti, ForceMode.Impulse);
            }

        }
    }

    public bool CanDoAbility()
    {
        if (Time.time > lastTimeAbility + abilityCooldown /*&& IsPlayerInClearSight()*/)
        {
            return true;
        }
        return false;
    }

    public void SetAbilityOnCooldown() => lastTimeAbility = Time.time;
    public void SetJumpAttackOnCooldown() => lastTimeJump = Time.time;

    public bool IsPlayerInClearSight()
    {
        Vector3 myPosition = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 playerPosition = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPosition - myPosition).normalized;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, 100, ~whatToIgnore))
        {
            if (hit.transform == player || hit.transform.parent == player)
                return true;
        }
        return false;
    }

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoint <= 0 && stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Vector3 myPosition = transform.position + new Vector3(0, 1.5f, 0);
            Vector3 playerPosition = player.position + Vector3.up;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(myPosition, playerPosition);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
