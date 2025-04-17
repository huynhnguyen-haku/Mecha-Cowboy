using UnityEngine;

public class Health_Pickup : Interactable
{
    [SerializeField] private int healthAmount = 50; // Amount of health to restore


    public override void Interact()
    {
        Player player = FindFirstObjectByType<Player>(); // Find the player in the scene
        if (player != null)
        {
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                RestoreHealth(playerHealth);
            }
        }
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void RestoreHealth(Player_Health playerHealth)
    {
        playerHealth.HealHeath(healthAmount);
        Debug.Log($"[Health_Pickup] Player restored {healthAmount} health.");
    }
}
