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

        interactionDisabled = false;
        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        enemy.ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();
        //DisableInteraction();
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
