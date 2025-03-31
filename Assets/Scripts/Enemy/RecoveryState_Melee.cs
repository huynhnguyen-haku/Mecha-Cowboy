public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(enemy.player.transform.position);
        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
    }
}