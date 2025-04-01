using UnityEngine;

public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Enemy_Ragdoll ragdoll;

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
        ragdoll = enemy.GetComponent<Enemy_Ragdoll>();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        ragdoll.RagdollActive(true);
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

