using UnityEngine;


public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;


    private float coverCheckTimer;
    private bool firstTimeAttack = true;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
        lastTimeShot = Time.realtimeSinceStartup;
    }

    public override void Enter()
    {
        base.Enter();
        SetupFirstAttack();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.visual.EnableIK(true, true);
        stateTimer = enemy.attackDelay;
    }

    public override void Update()
    {
        base.Update();


        if (enemy.IsSeeingPlayer())
        {
            enemy.FaceTarget(enemy.aim.position);
        }

        if (enemy.CanThrowGrenade())
        {
            stateMachine.ChangeState(enemy.throwGrenadeState);
        }

        if (HandleAdvancePlayer())
        {
            stateMachine.ChangeState(enemy.advancePlayerState);
        }

        ChangeCoverIfShould();

        if (stateTimer > 0)
        {
            return;
        }


        if (WeaponOutOfBullets())
        {
            if (enemy.IsUnstoppable() && UnStoppableWalkReady())
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState);
            }

            if (WeaponOnCooldown())
            {
                AttempToResetWeapon();
            }

            return;
        }

        if (CanShoot() && enemy.IsAimingOnPlayer())
        {
            Shoot();
        }

    }

    private bool HandleAdvancePlayer()
    {
        if (enemy.IsUnstoppable())
        {
            return false;
        }

        return (enemy.IsPlayerInAgrressionRage() == false && ReadyToLeaveCover());

    }
    private bool ReadyToLeaveCover()
    {
        return Time.time > enemy.coverTime + enemy.runToCoverState.lastTimeTookCover;
    }


    #region Weapon System

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
    private void SetupFirstAttack()
    {
        if (firstTimeAttack)
        {
            firstTimeAttack = false;
            bulletsPerAttack = enemy.weaponData.GetRandomBulletPerAttack();
            weaponCooldown = enemy.weaponData.GetRandomWeaponCooldown();
        }
    }

    #endregion

    #region Cover System
    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || IsPlayerClose();
        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration;

        return inDanger && advanceTimeIsOver;
    }

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;
        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.transform.parent == enemy.player;
        }
        return false;
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

        if (ReadyToChangeCover() && ReadyToLeaveCover())
        {
            if (enemy.CanGetCover())
            {
                stateMachine.ChangeState(enemy.runToCoverState);
            }
        }
    }

    public bool UnStoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unStoppableWalkOnColdDown =
            Time.time < enemy.weaponData.minWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && unStoppableWalkOnColdDown == false;
    }
    #endregion
}
