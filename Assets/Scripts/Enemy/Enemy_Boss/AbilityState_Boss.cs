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
        enemy.bossVisual.EnableWeaponTrail(true);
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.player.position);

        if (ShouldDisableFlamethrower())
            DisableFlamethrower();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    private bool ShouldDisableFlamethrower()
    {
        return stateTimer < 0;
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        if (enemy.weaponType == BossWeaponType.Flamethrower)
        {
            enemy.ActivateFlamethrower(true);
            enemy.bossVisual.DischargeBatteries();
            enemy.bossVisual.EnableWeaponTrail(false);

            // Kích ho?t m? r?ng collider
            Flamethrower_DamageArea damageArea = enemy.flamethrower.GetComponentInChildren<Flamethrower_DamageArea>();
            if (damageArea != null)
            {
                damageArea.StartExpandingCollider();
            }
        }

        if (enemy.weaponType == BossWeaponType.Hammer)
        {
            enemy.ActivateHammer();
            enemy.bossVisual.EnableWeaponTrail(true);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetAbilityOnCooldown();
        enemy.bossVisual.ResetBatteries();
        enemy.bossVisual.EnableWeaponTrail(false);
    }

    public void DisableFlamethrower()
    {
        if (enemy.weaponType != BossWeaponType.Flamethrower)
        {
            return;
        }

        if (enemy.flamethrowerActive == false)
        {
            return;
        }
        enemy.ActivateFlamethrower(false);
    }
}
