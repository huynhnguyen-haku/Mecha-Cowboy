using UnityEngine;

public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Melee melee;
    private Enemy_Boss boss;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        melee = GetComponentInParent<Enemy_Melee>();
        boss = GetComponentInParent<Enemy_Boss>();
    }

    public void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMovement() => enemy.ActivateManualMovement(false);


    public void StartManualRotation() => enemy.ActivateManualRotation(true);
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    public void AbilityEvent()
    {
        enemy.AbilityTrigger();
    }
    public void EnableIK() => enemy.visual.EnableIK(true, true, 1f);

    public void EnableWeaponModel()
    {
        enemy.visual.EnableWeaponModel(true);
        enemy.visual.EnableHoldingWeaponModel(false);
    }

    public void BossJumpImpact()
    {
        boss?.JumpImpact();
    }

    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttack(true);
    }

    public void FinishMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttack(false);
    }
}
