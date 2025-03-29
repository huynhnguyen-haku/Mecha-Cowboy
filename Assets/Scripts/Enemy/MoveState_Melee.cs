using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint());

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    private Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemy.agent;
        NavMeshPath path = agent.path;  

        if (path.corners.Length < 2)
        {
            return agent.destination;
        }

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
            {
                return path.corners[i + 1];
            }
        }
        return agent.destination;
    }
}
