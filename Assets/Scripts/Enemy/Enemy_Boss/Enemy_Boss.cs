using UnityEngine;

public class Enemy_Boss : Enemy
{
    [Header("Jump Attack")]
    public float travelTimeToTarget = 1;
    public float jumpAttackCooldown = 10;
    private float lastTimeJump;
    public float minJumpDistanceRequired;

    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    [Space]
    [SerializeField] private LayerMask whatToIgnore;

    public float attackRange;

    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
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
        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < minJumpDistanceRequired)
            return false;

        if (Time.time > lastTimeJump + jumpAttackCooldown && IsPlayerInClearSight())
        {
            lastTimeJump = Time.time;
            return true;
        }
        return false;
    }

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
    }
}
