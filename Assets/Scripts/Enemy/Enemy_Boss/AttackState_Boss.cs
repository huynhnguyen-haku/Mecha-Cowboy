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

        // Randomize the attack animation based on the number of attack animations available
        enemy.anim.SetFloat("AttackIndex", Random.Range(0, enemy.attackAnimationCount));
        enemy.agent.isStopped = true;

        stateTimer = 1f;
        enemy.bossVisual.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            enemy.FaceTarget(enemy.player.position, 20);

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.idleState);
            else
                stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        lastTimeAttack = Time.time;
        enemy.bossVisual.EnableWeaponTrail(false);
    }
    #endregion
}