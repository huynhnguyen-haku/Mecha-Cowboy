using UnityEngine;

public class Player_Health : HealthController
{
    private Player player;
    private Player_AimController aim;

    public bool playerIsDead; 
    private LineRenderer aimLaser;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        aimLaser = player.aim.aimLaser;
    }

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);
        if (PlayerShouldDie())
        {
            Die();
        }
    }

    private void Die()
    {
        if (playerIsDead) 
            return;

        playerIsDead = true;
        player.anim.enabled = false;
        player.ragdoll.RagdollActive(true);
        player.aim.aimLaser.enabled = false;
    }
}
