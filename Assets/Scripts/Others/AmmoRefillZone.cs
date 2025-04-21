using UnityEngine;

public class AmmoRefillZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Get the Player_WeaponController component from the player
            Player_WeaponController weaponController = other.GetComponent<Player_WeaponController>();

            if (weaponController != null)
            {
                // Get the current weapon the player is using
                Weapon currentWeapon = weaponController.CurrentWeapon();

                if (currentWeapon != null)
                {
                    // Continuously refill the Total Reserve Ammo to 999
                    currentWeapon.TotalReserveAmmo = Mathf.Min(999, currentWeapon.TotalReserveAmmo + 1);
                }
            }
        }
    }
}
