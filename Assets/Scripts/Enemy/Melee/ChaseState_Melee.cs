using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    private float lstTimeUpdateDestination;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.chaseSpeed;
        enemy.agent.isStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }

        enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.SetDestination(enemy.player.transform.position);
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lstTimeUpdateDestination + 0.25f)
        {
            lstTimeUpdateDestination = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }
}

