using UnityEngine;

public class Player_Health : HealthController
{
    public override void ReduceHealth()
    {
        base.ReduceHealth();

        if (ShouldDie())
        {
            Die();
        }

    }

    private void Die()
    {

    }
}
