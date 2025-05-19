using UnityEngine;

public class IdleState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        // If player is in attack range and enemy is in battle mode, change to attack state
        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        // Change to move state when idle time is over
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }
    #endregion
}