using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private float lastTimeUpdateDestination;

    private float footstepTimer; // Bộ đếm thời gian cho footstep
    private float footstepInterval; // Khoảng thời gian giữa các bước chân

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.isStopped = false;

        footstepInterval = CalculateFootstepInterval(enemy.agent.speed); // Calculate the time between footsteps
        footstepTimer = 0f; // Reset the timer
        PlayFootstepSFX();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);

        enemy.FaceTarget(enemy.agent.steeringTarget);

        if (CanUpdateDestination())
            enemy.agent.SetDestination(enemy.player.transform.position);

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

    private void PlayFootstepSFX()
    {
        // Dừng âm thanh đi bộ nếu đang phát
        if (enemy.meleeSFX.walkSFX.isPlaying)
            enemy.meleeSFX.walkSFX.Stop();

        // Phát âm thanh chạy nếu chưa phát
        if (!enemy.meleeSFX.runSFX.isPlaying)
            enemy.meleeSFX.runSFX.PlayOneShot(enemy.meleeSFX.runSFX.clip);
    }


    private float CalculateFootstepInterval(float speed)
    {
        return Mathf.Clamp(1f / speed, 0.1f, 0.1f);
    }
}

