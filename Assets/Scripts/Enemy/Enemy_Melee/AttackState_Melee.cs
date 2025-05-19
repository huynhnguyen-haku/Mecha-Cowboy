using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    [Header("Attack Parameters")]
    private Vector3 attackDirection; // Direction for the enemy to move during the attack
    private float attackMoveSpeed; // Speed at which the enemy moves during the attack
    private const float MAX_ATTACK_DISTANCE = 50f; // Maximum distance of charge attack

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();

        enemy.UpdateAttackData();
        enemy.visual.EnableWeaponModel(true);
        enemy.visual.EnableWeaponTrail(true);

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);

        // Randomize the attack animation based on the number of attack animations available
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0, 6));

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                // Change to recovery state, to determine next action
                // Such as throwing an axe, continue attacking, or chase
                stateMachine.ChangeState(enemy.recoveryState);

            else
                // Chase the player if not in attack range
                stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Set up next attack by using the attack data
        SetupNextAttack(); 
        enemy.visual.EnableWeaponTrail(false);
    }
    #endregion

    #region Attack Setup Methods
    private void SetupNextAttack()
    {
        int recoveryIndex = PlayerClose() ? 1 : 0;
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);
        enemy.attackData = UpdatedAttackData();
    }

    // Check if the player is close (within 1 unit)
    private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.position) <= 1;

    // Select a new attack data, excluding charge attacks if player is close
    private AttackData_EnemyMelee UpdatedAttackData()
    {
        List<AttackData_EnemyMelee> validAttacks = new List<AttackData_EnemyMelee>(enemy.attackList);
        if (PlayerClose())
            validAttacks.RemoveAll(parameter => parameter.attackType == AttackType_Melee.Charge);

        int random = Random.Range(0, validAttacks.Count);
        return validAttacks[random];
    }
    #endregion
}