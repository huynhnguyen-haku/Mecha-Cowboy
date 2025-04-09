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

        interactionDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
    }


    public override void Update()
    {
        base.Update();
        DisableInteraction();
    }

    private void DisableInteraction()
    {
        if (stateTimer <= 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            enemy.ragdoll.RagdollActive(false);
            enemy.ragdoll.ColliderActive(false);
        }
    }
}