using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public RecoveryState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
    }
    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.transform.position);

        if (triggerCalled)
        {
            if (enemy.CanThrowAxe())
                // Change to ability state if the axe is available
                stateMachine.ChangeState(enemy.abilityState);

            else if (enemy.PlayerInAttackRange())
                // Change to attack state if the player is in attack range
                stateMachine.ChangeState(enemy.attackState);

            else
                // Chase the player if not in attack range
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    #endregion
}