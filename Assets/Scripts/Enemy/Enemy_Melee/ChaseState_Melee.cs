using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    [Header("Chase State Parameters")]
    private float lastTimeUpdateDestination; // Tracks the last time the destination was updated
    private float footstepTimer; // Timer for footstep sound effects
    private float footstepInterval; // Interval between footstep sounds

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.isStopped = false;

        footstepInterval = CalculateFootstepInterval(enemy.agent.speed); // Calculate the time between footsteps
        footstepTimer = 0f; // Reset the timer
        PlayFootstepSFX();
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.agent.steeringTarget);

        if (enemy.PlayerInAttackRange())
            // Switch to the attack state
            stateMachine.ChangeState(enemy.attackState);

        if (CanUpdateDestination())
            // If the destination can be updated, set it to the player's position
            enemy.agent.SetDestination(enemy.player.transform.position);

        HandleFootstepSFX();
    }

    public override void Exit()
    {
        base.Exit();
    }
    #endregion

    #region Footstep Sound Effects
    // Manage footstep sound effects timing
    private void HandleFootstepSFX()
    {
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= footstepInterval)
        {
            footstepTimer = 0f;
            PlayFootstepSFX();
        }
    }

    // Play the appropriate footstep sound effect for running
    private void PlayFootstepSFX()
    {
        // Stop walk sound if playing
        if (enemy.meleeSFX.walkSFX.isPlaying)
            enemy.meleeSFX.walkSFX.Stop();

        // Play run sound if not already playing
        if (!enemy.meleeSFX.runSFX.isPlaying)
            enemy.meleeSFX.runSFX.PlayOneShot(enemy.meleeSFX.runSFX.clip);
    }

    private float CalculateFootstepInterval(float speed)
    {
        return Mathf.Clamp(1f / speed, 0.1f, 0.1f);
    }
    #endregion

    #region Destination Logic
    // Determine if the destination should be updated
    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdateDestination + 0.25f)
        {
            lastTimeUpdateDestination = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}