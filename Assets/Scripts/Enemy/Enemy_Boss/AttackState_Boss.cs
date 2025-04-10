using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetFloat("AttackIndex", Random.Range(0, 2));
        enemy.agent.isStopped = true;

    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            else
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }
}
