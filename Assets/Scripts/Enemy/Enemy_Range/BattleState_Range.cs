using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;
    private float aimStartTime;
    private float aimDelay = 1.0f; // Delay before shooting after facing the player

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
        lastTimeShot = Time.realtimeSinceStartup;
    }

    public override void Enter()
    {
        base.Enter();
        bulletsPerAttack = enemy.weaponData.GetRandomBulletPerAttack();
        weaponCooldown = enemy.weaponData.GetRandomWeaponCooldown();

        enemy.agent.isStopped = true;
        enemy.enemyVisual.EnableIK(true, true);
        aimStartTime = Time.time; // Record the time when the enemy starts aiming
    }

    public override void Exit()
    {
        base.Exit();
        enemy.enemyVisual.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (Time.time < aimStartTime + aimDelay)
        {
            // Wait for the aim delay before allowing to shoot
            return;
        }

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
        bulletsPerAttack = enemy.weaponData.GetRandomBulletPerAttack();
        weaponCooldown = enemy.weaponData.GetRandomWeaponCooldown();
    }

    private bool WeaponOnCooldown()
    {
        return Time.time > lastTimeShot + weaponCooldown;
    }
    private bool WeaponOutOfBullets()
    {
        return bulletsShoot >= bulletsPerAttack;
    }

    private bool CanShoot()
    {
        float timeBetweenShots = 60f / enemy.weaponData.fireRate;
        return Time.time > lastTimeShot + timeBetweenShots;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShoot++;
    }
}
