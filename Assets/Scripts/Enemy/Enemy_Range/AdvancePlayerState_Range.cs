using UnityEngine;

public class AdvancePlayerState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPosition;

    public AdvancePlayerState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.enemyVisual.EnableIK(true, false);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;
    }

    public override void Update()
    {
        base.Update();

        playerPosition = enemy.player.transform.position;

        enemy.agent.SetDestination(enemy.player.transform.position);
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, playerPosition) < enemy.arrgresssionRange)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
