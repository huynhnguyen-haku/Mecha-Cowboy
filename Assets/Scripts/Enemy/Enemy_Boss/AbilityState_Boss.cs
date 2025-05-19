using UnityEngine;

public class AbilityState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    public AbilityState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Boss)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        stateTimer = enemy.flamethrowDuration;
        enemy.bossVisual.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();

        // Face the player while activating the ability
        enemy.FaceTarget(enemy.player.position);

        if (ShouldDisableFlamethrower())
            DisableFlamethrower();

        // Active animation trigger (in animation) to change state
        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();

        // Reset the ability cooldown and recharge the batteries
        enemy.SetAbilityOnCooldown();
        enemy.bossVisual.ResetBatteries();
        enemy.bossVisual.EnableWeaponTrail(false);
    }
    #endregion

    #region Ability Trigger Logic
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        // For flamethrower boss: Activate flamethrower, discharge batteries, and expand damage area
        if (enemy.weaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisual.DischargeBatteries();
            enemy.bossVisual.EnableWeaponTrail(false);

            // Enable the damage area of the flamethrower
            Flamethrower_DamageArea damageArea = enemy.flamethrower.GetComponentInChildren<Flamethrower_DamageArea>();
            if (damageArea != null)
            {
                damageArea.StartExpandingCollider();
            }
        }

        // For hammer boss: Activate hammer and enable weapon trail
        if (enemy.weaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
            enemy.bossVisual.EnableWeaponTrail(true);
        }
    }
    #endregion

    #region Flamethrower Control Methods
    private bool ShouldDisableFlamethrower()
    {
        return stateTimer < 0;
    }

    public void DisableFlamethrower()
    {
        if (enemy.weaponType != BossWeaponType.Flamethrower)
            return;

        if (enemy.flamethrowerActive == false)
            return;

        enemy.ActivateFlamethrower(false);
    }
    #endregion
}