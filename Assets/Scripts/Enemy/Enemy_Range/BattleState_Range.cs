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

    private float coverCheckTimer;

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

        ChangeCoverIfShould();

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

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.ChangeCover)
            return;


        // Check the cover every 0.5 seconds
        coverCheckTimer -= Time.deltaTime;
        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 0.5f;
        }

        if (IsPlayerInClearSight() || IsPlayerClose())
        {
            if (enemy.CanGetCover())
            {
                stateMachine.ChangeState(enemy.runToCoverState);
            }
        }
    }

    #region Weapon

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

    #endregion

    #region Cover System

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;
        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }
        return false;
    }
    #endregion
}
