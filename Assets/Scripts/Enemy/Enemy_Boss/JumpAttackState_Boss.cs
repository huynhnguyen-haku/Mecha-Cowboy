using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    [Header("Jump Attack")]
    private Vector3 lastPlayerPosition; // Stores the player's position at the start of the jump attack
    private float jumpAttackMovementSpeed; // Speed at which the boss moves toward the player during the jump attack

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();

        // Get the player's last position for targeting
        lastPlayerPosition = enemy.player.position;

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        // Place the landing zone effect at the player's position and enable weapon trail
        enemy.bossVisual.PlaceLandingZone(lastPlayerPosition);
        enemy.bossVisual.EnableWeaponTrail(true);

        // Calculate the jump attack movement speed based on the distance to the player
        float distanceToPlayer = Vector3.Distance(lastPlayerPosition, enemy.transform.position);
        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;
        enemy.FaceTarget(lastPlayerPosition, 1000);

        // For hammer boss: Use NavMeshAgent to move toward the player
        if (enemy.weaponType == BossWeaponType.Hammer)
        {
            enemy.agent.isStopped = false;
            enemy.agent.speed = enemy.runSpeed;
            enemy.agent.SetDestination(lastPlayerPosition);
        }
    }

    public override void Update()
    {
        base.Update();

        Vector3 myPosition = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive();

        // Use manual movement if active, otherwise rely on NavMeshAgent
        if (enemy.ManualMovementActive())
        {
            enemy.agent.velocity = Vector3.zero;
            enemy.transform.position = Vector3.MoveTowards(myPosition, lastPlayerPosition, jumpAttackMovementSpeed * Time.deltaTime);
        }

        // Transition to move state when the jump attack is complete
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Cool down the jump attack ability and disable weapon trail
        enemy.SetJumpAttackOnCooldown();
        enemy.bossVisual.EnableWeaponTrail(false);
    }
    #endregion
}