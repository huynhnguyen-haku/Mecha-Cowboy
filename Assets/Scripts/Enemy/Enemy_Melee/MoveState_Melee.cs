using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    [Header("Move State Parameters")]
    private Vector3 destination; // The destination for the enemy's patrol movement
    private float footstepTimer; // Timer for footstep sound effects
    private float footstepInterval; // Interval between footstep sounds

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.walkSpeed;

        // Setup the enemy's patrol point
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

        footstepInterval = CalculateFootstepInterval(enemy.agent.speed);
        footstepTimer = 0f;
        PlayFootstepSFX();
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(enemy.agent.steeringTarget);

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
        {
            // Switch to idle state when reaching the patrol point
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        HandleFootstepSFX();
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

    // Play the appropriate footstep sound effect for walking
    private void PlayFootstepSFX()
    {
        // Stop run sound if playing
        if (enemy.meleeSFX.runSFX.isPlaying)
            enemy.meleeSFX.runSFX.Stop();

        // Play walk sound if not already playing
        if (!enemy.meleeSFX.walkSFX.isPlaying)
            enemy.meleeSFX.walkSFX.PlayOneShot(enemy.meleeSFX.walkSFX.clip);
    }

    // Calculate the interval between footstep sounds based on speed
    private float CalculateFootstepInterval(float speed)
    {
        return Mathf.Clamp(1f / speed, 0.12f, 0.12f);
    }
    #endregion
}