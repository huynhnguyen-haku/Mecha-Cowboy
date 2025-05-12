using UnityEngine;

public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy;

    public DeadState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        if (enemy.throwGrenadeState.finishedThrowing == false)
        {
            enemy.ThrowGrenade();
        }
        stateTimer = 1.5f;

        // Stop all footstep sounds
        if (enemy.rangeSFX.walkSFX.isPlaying)
            enemy.rangeSFX.walkSFX.Stop();

        if (enemy.rangeSFX.runSFX.isPlaying)
            enemy.rangeSFX.runSFX.Stop();
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