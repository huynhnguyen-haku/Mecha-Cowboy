using UnityEngine;

public class DeadState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private bool interactionDisabled;

    public DeadState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.abilityState.DisableFlamethrower();
        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();
    }
}
