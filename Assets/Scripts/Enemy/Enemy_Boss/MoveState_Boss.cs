using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;

    private float actionTimer;
    private float timeBeforeSpeedUp = 5;
    private bool SpeedUpActive;

    private float footstepTimer; // Bộ đếm thời gian cho footstep
    private float footstepInterval; // Khoảng thời gian giữa các bước chân

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        SpeedReset();
        enemy.agent.isStopped = false;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
        actionTimer = enemy.actionCooldown;

        footstepInterval = CalculateFootstepInterval(enemy.agent.speed); // Tính khoảng thời gian giữa các bước chân
        footstepTimer = 0f; // Đặt lại bộ đếm thời gian
    }

    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(enemy.agent.steeringTarget);

        if (enemy.inBattleMode)
        {
            if (ShouldSpeedUp())
            {
                SpeedUp();
            }

            Vector3 playerPosition = enemy.player.position;
            enemy.agent.SetDestination(playerPosition);

            if (actionTimer < 0)
            {
                PerfomRandomAction();
            }
            else if (enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        HandleFootstepSFX(); // Xử lý âm thanh bước chân
    }

    private void HandleFootstepSFX()
    {
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= footstepInterval)
        {
            footstepTimer = 0f;
            PlayFootstepSFX();
        }
    }

    private void PlayFootstepSFX()
    {
        if (SpeedUpActive)
        {
            enemy.bossSFX.runSFX.PlayOneShot(enemy.bossSFX.runSFX.clip); // Phát âm thanh chạy
        }
        else
        {
            enemy.bossSFX.walkSFX.PlayOneShot(enemy.bossSFX.walkSFX.clip); // Phát âm thanh đi bộ
        }
    }

    private float CalculateFootstepInterval(float speed)
    {
        // Tính khoảng thời gian giữa các bước chân dựa trên tốc độ di chuyển
        return Mathf.Clamp(1f / speed, 0.3f, 0.5f); // Ví dụ: tốc độ càng cao thì khoảng cách giữa các bước chân càng ngắn
    }

    private void SpeedReset()
    {
        SpeedUpActive = false;
        enemy.anim.SetFloat("MoveIndex", 0); // Set the move index to 0, so boss will walk normally
        enemy.agent.speed = enemy.walkSpeed;

        footstepInterval = CalculateFootstepInterval(enemy.walkSpeed); // Cập nhật khoảng thời gian bước chân
    }

    private void SpeedUp()
    {
        SpeedUpActive = true;
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("MoveIndex", 1); // Set the move index to 1, so boss will run

        footstepInterval = CalculateFootstepInterval(enemy.runSpeed); // Cập nhật khoảng thời gian bước chân
    }

    private void PerfomRandomAction()
    {
        actionTimer = enemy.actionCooldown;

        if (Random.Range(0, 2) == 0)
        {
            ActiveSpecialAbility();
        }
        else
        {
            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState);
            // For hammer boss only
            else if (enemy.weaponType == BossWeaponType.Hammer)
                ActiveSpecialAbility();
        }
    }

    private void ActiveSpecialAbility()
    {
        if (enemy.CanDoAbility())
            stateMachine.ChangeState(enemy.abilityState);
    }

    private bool ShouldSpeedUp()
    {
        if (SpeedUpActive)
        {
            return false;
        }

        if (Time.time > enemy.attackState.lastTimeAttack + timeBeforeSpeedUp)
        {
            return true;
        }
        return false;
    }
}
