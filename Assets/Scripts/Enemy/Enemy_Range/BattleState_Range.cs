using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot;
    private int bulletsShoot = 0;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
        lastTimeShot = Time.realtimeSinceStartup;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
        enemy.enemyVisual.EnableIK(true);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.enemyVisual.EnableIK(false);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
            {
                AttempToResetWeapon();
            }
            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttempToResetWeapon()
    {
        bulletsShoot = 0;
    }

    private bool WeaponOnCooldown()
    {
        return Time.time > lastTimeShot + enemy.weaponCooldown;
    }
    private bool WeaponOutOfBullets()
    {
        return bulletsShoot >= enemy.bulletsToShoot;
    }

    private bool CanShoot()
    {
        float timeBetweenShots = 60f / enemy.fireRate;
        return Time.time > lastTimeShot + timeBetweenShots;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShoot++;
    }
}
