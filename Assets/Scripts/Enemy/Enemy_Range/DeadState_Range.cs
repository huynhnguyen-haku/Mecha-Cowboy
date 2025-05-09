using UnityEngine;

public class DeadState_Range : EnemyState
{
    private Enemy_Range enemy;

    private bool interactionDisabled;

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