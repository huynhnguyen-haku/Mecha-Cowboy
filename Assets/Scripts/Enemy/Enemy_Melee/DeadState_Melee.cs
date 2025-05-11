public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    private bool interactionDisabled;

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1.5f;

        // Dừng tất cả âm thanh bước chân
        if (enemy.meleeSFX.walkSFX.isPlaying)
            enemy.meleeSFX.walkSFX.Stop();

        if (enemy.meleeSFX.runSFX.isPlaying)
            enemy.meleeSFX.runSFX.Stop();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}

