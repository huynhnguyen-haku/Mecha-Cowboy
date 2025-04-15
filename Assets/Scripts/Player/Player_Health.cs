using UnityEngine;

public class Player_Health : HealthController
{
    private Player player;
    private PlayerAim aim;

    public bool isDead;
    public LineRenderer aimLaser;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        aimLaser = player.aim.aimLaser;
    }

    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);
        if (ShouldDie())
        {
            Die();
        }

    }

    private void Die()
    {
        isDead = true;
        player.anim.enabled = false;
        player.ragdoll.RagdollActive(true);
        player.aim.aimLaser.enabled = false; // Disable aim laser on death
    }
}
