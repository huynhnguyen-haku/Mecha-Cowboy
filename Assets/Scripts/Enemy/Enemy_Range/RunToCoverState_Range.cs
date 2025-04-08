using UnityEngine;

public class RunToCoverState_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 destination;

    public float lastTimeTookCover { get; private set; }

    public RunToCoverState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Range)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemy.currentCover.transform.position;

        enemy.visual.EnableIK(true, false);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runSpeed;
        enemy.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
        lastTimeTookCover = Time.time;
    }

    public override void Update()
    {
        base.Update();
        enemy.FaceTarget(GetNextPathPoint());

        if (Vector3.Distance(enemy.transform.position, destination) < 0.5f)
        {
            Debug.Log("Reached cover");
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
