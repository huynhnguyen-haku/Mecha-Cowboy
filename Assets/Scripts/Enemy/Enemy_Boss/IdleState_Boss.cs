public class IdleState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public IdleState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.inBattleMode && enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
