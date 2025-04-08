using UnityEngine;

public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy;
    public ThrowGrenadeState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.visual.EnableWeaponModel(false);
        enemy.visual.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.position);
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ThrowGrenade();
    }
}
