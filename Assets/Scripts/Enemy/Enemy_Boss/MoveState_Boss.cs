using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;

    [Header("Move State")]
    private Vector3 destination; // The destination for the boss to move toward
    private float actionTimer; // Timer for performing random actions
    private float timeBeforeSpeedUp = 5f; // Time before the boss speeds up when chasing the player
    private bool SpeedUpActive; // Tracks if the boss is in speed-up mode

    private float footstepTimer; // Timer for footstep sound effects
    private float footstepInterval; // Interval between footstep sounds

    public MoveState_Boss(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        this.enemy = (Enemy_Boss)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        SpeedReset();
        enemy.agent.isStopped = false;

        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
        actionTimer = enemy.actionCooldown;

        footstepInterval = CalculateFootstepInterval(enemy.agent.speed);
        footstepTimer = 0f; // Reset the footstep timer
    }

    public override void Update()
    {
        base.Update();
        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(enemy.agent.steeringTarget);

        if (enemy.inBattleMode)
        {
            // In battle mode: Chase the player and perform actions
            if (ShouldSpeedUp())
                SpeedUp();

            Vector3 playerPosition = enemy.player.position;
            enemy.agent.SetDestination(playerPosition);

            if (actionTimer < 0)
                // Perform a random action: jump attack or special ability
                PerfomRandomAction();

            else if (enemy.PlayerInAttackRange())
                // Switch to the attack state
                stateMachine.ChangeState(enemy.attackState);
        }
        else
        {
            // Patrol mode: Change to idle state when close to destination
            if (Vector3.Distance(enemy.transform.position, destination) < 0.25f)
                stateMachine.ChangeState(enemy.idleState);
        }

        HandleFootstepSFX();
    }
    #endregion

    #region Footstep Sound Effects
    private void HandleFootstepSFX()
    {
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            footstepTimer = 0f;
            PlayFootstepSFX();
        }
    }

    // Play the appropriate footstep sound effect based on the current speed
    private void PlayFootstepSFX()
    {
        if (SpeedUpActive)
        {
            enemy.bossSFX.walkSFX.Stop();
            enemy.bossSFX.runSFX.PlayOneShot(enemy.bossSFX.runSFX.clip);
        }
        else
        {
            enemy.bossSFX.runSFX.Stop();
            enemy.bossSFX.walkSFX.PlayOneShot(enemy.bossSFX.walkSFX.clip);
        }
    }

    // Calculate the interval between footstep sounds based on speed
    private float CalculateFootstepInterval(float speed)
        => Mathf.Clamp(1f / speed, 0.3f, 0.5f);
    #endregion

    #region Speed Control Methods
    private void SpeedReset()
    {
        SpeedUpActive = false;
        enemy.anim.SetFloat("MoveIndex", 0); // Set the move index to 0 for walking animation
        enemy.agent.speed = enemy.walkSpeed;

        footstepInterval = CalculateFootstepInterval(enemy.walkSpeed);
    }

    private void SpeedUp()
    {
        SpeedUpActive = true;
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("MoveIndex", 1); // Set the move index to 1 for running animation

        footstepInterval = CalculateFootstepInterval(enemy.runSpeed);
    }

    // Determine if the boss should speed up based on time since last attack
    private bool ShouldSpeedUp()
    {
        if (SpeedUpActive)
            return false;

        if (Time.time > enemy.attackState.lastTimeAttack + timeBeforeSpeedUp)
            return true;

        return false;
    }
    #endregion

    #region Action Logic
    private void PerfomRandomAction()
    {
        actionTimer = enemy.actionCooldown;

        if (Random.Range(0, 2) == 0)
            ActiveSpecialAbility(); // 50% chance to perform a special ability
        else
        {
            if (enemy.CanDoJumpAttack())
                stateMachine.ChangeState(enemy.jumpAttackState); // Prioritize jump attack if possible

            else
            {
                switch (enemy.weaponType)
                {
                    case BossWeaponType.Hammer:
                        ActiveSpecialAbility(); // Perform hammer special ability
                        break;

                    case BossWeaponType.Flamethrower:
                        ActiveSpecialAbility(); // Perform flamethrower special ability
                        break;
                }
            }
        }
    }

    private void ActiveSpecialAbility()
    {
        if (enemy.CanDoAbility())
            stateMachine.ChangeState(enemy.abilityState);
    }
    #endregion
}