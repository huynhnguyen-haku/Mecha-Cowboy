using System.Threading;
using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        stateTimer = enemy.flamethrowDuration;
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.position);

        if (stateTimer < 0 && enemy.flamethrowerActive)
        {
            enemy.ActivateFlamethrower(false);
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ActivateFlamethrower(true);
    }
}
