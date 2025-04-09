using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.walkSpeed;
        enemy.agent.isStopped = false;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.inBattleMode)
        {
            Vector3 playerPosition = enemy.player.position;
            enemy.agent.SetDestination(playerPosition);

            if (Vector3.Distance(playerPosition, enemy.transform.position) < enemy.attackRange)
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }

        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

    }
}
