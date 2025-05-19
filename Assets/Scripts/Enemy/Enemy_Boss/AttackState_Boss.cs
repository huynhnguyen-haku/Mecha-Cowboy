using UnityEngine;

public class AttackState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public float lastTimeAttack { get; private set; }

    public AttackState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        enemy.bossVisual.EnableWeaponTrail(true);

        // Randomize the attack animation based on the number of attack animations available
        enemy.anim.SetFloat("AttackIndex", Random.Range(0, enemy.attackAnimationCount));
        enemy.agent.isStopped = true;
        stateTimer = 1f;
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer > 0)
            enemy.FaceTarget(enemy.player.position, 20);

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                // Change to idle state, then to attack state again
                stateMachine.ChangeState(enemy.idleState);

            else
                // Chase the player if not in attack range
                stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Store the last attack time
        // This is used to speed up the boss if conditions meet
        lastTimeAttack = Time.time;
        enemy.bossVisual.EnableWeaponTrail(false);
    }
    #endregion
}