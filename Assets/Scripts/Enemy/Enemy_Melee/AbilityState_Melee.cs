using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    [Header("Ability Parameters")]
    private Vector3 movementDirection; // Direction for the enemy to move during the ability
    private float moveSpeed; // Speed at which the enemy moves during the ability
    private const float MAX_MOVEMENT_DISTANCE = 20f; // Maximum distance of the ability

    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = (Enemy_Melee)enemyBase;
    }

    #region State Lifecycle Methods
    public override void Enter()
    {
        base.Enter();

        // Enable the weapon mode, change movespeed to its walk speed
        // Limit the movement distance while activating the ability
        enemy.visual.EnableWeaponModel(true);
        moveSpeed = enemy.walkSpeed;
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (enemy.ManualMovementActive())
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.walkSpeed * Time.deltaTime);

        // Change to recovery state when the ability is complete
        if (triggerCalled)
            stateMachine.ChangeState(enemy.recoveryState);
    }

    public override void Exit()
    {
        base.Exit();

        // Reset speed and animation parameters
        enemy.walkSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
    }
    #endregion

    #region Ability Trigger Logic
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        enemy.ThrowAxe();
    }
    #endregion
}