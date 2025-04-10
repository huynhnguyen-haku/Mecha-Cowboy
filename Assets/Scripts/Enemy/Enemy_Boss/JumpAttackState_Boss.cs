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

        float distanceToPlayer = Vector3.Distance(lastPlayerPosition, enemy.transform.position);

        jumpAttackMovementSpeed = distanceToPlayer / enemy.travelTimeToTarget;
        enemy.FaceTarget(lastPlayerPosition, 1000);
    }

    public override void Update()
    {
        base.Update();
        Vector3 myPosition = enemy.transform.position;
        enemy.agent.enabled = !enemy.ManualMovementActive();

        if (enemy.ManualMovementActive())
        {
          enemy.transform.position =  Vector3.MoveTowards(myPosition, lastPlayerPosition, jumpAttackMovementSpeed * Time.deltaTime);
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
