using UnityEngine;

public class Item_Pickup : MonoBehaviour
{
    [SerializeField] private Weapon_Data weapon_Data;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<WeaponController>()?.PickupWeapon(weapon_Data);
    }
}
