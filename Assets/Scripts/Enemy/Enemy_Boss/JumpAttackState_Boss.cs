using UnityEngine;

public class JumpAttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 lastPlayerPosition;

    private float jumpAttackMovementSpeed;

    public JumpAttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        lastPlayerPosition = enemy.player.position;
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;    

        enemy.bossVisual.PlaceLandingZone(lastPlayerPosition); // Place the landing zone effect at the player's position
        enemy.bossVisual.EnableWeaponTrail(true); // Enable the weapon trail effect

        float distanceToPlayer = Vector3.Distance(lastPlayerPosition, enemy.transform.position);

        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;
        enemy.FaceTarget(lastPlayerPosition, 1000);

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

        if (enemy.ManualMovementActive())
        {
            enemy.agent.velocity = Vector3.zero;
            enemy.transform.position =  Vector3.MoveTowards(myPosition, lastPlayerPosition, jumpAttackMovementSpeed * Time.deltaTime);
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetJumpAttackOnCooldown();
        enemy.bossVisual.EnableWeaponTrail(false); // Disable the weapon trail effect
    }
}
