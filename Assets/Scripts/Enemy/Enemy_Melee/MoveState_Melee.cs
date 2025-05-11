using UnityEngine;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;

    private float footstepTimer;
    private float footstepInterval;

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.walkSpeed;
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
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        HandleFootstepSFX();
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
        // Dừng âm thanh chạy nếu đang phát
        if (enemy.meleeSFX.runSFX.isPlaying)
            enemy.meleeSFX.runSFX.Stop();

        // Phát âm thanh đi bộ nếu chưa phát
        if (!enemy.meleeSFX.walkSFX.isPlaying)
            enemy.meleeSFX.walkSFX.PlayOneShot(enemy.meleeSFX.walkSFX.clip);
    }


    private float CalculateFootstepInterval(float speed)
    {
        return Mathf.Clamp(1f / speed, 0.3f, 0.5f);
    }
}
