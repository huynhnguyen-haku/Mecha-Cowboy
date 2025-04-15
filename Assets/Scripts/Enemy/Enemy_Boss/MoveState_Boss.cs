using UnityEngine;

public class MoveState_Boss : EnemyState
{
    private Enemy_Boss enemy;
    private Vector3 destination;

    private float actionTimer;
    private float timeBeforeSpeedUp = 5;
    private bool SpeedUpActive;

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
    }


    public override void Update()
    {
        base.Update();

        actionTimer -= Time.deltaTime;
        enemy.FaceTarget(GetNextPathPoint());

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
    }

    private void SpeedReset()
    {
        SpeedUpActive = false;
        enemy.anim.SetFloat("MoveIndex", 0); // Set the move index to 0, so boss will walk nomally
        enemy.anim.SetFloat("MoveSpeedMultipler", 1);
        enemy.agent.speed = enemy.walkSpeed;
    }
    private void SpeedUp()
    {
        SpeedUpActive = true;
        enemy.agent.speed = enemy.runSpeed;
        enemy.anim.SetFloat("MoveIndex", 1); // Set the move index to 1, so boss will run
        enemy.anim.SetFloat("MoveSpeedMultipler", 1.5f);
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
