using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player_WeaponVisuals weaponVisualController;
    private Player_WeaponController weaponController;

    private void Start()
    {
        weaponVisualController = GetComponentInParent<Player_WeaponVisuals>();
        weaponController = GetComponentInParent<Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();

        weaponController.SetWeaponReady(true);
    }
    public void ReturnRig()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponVisualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquippingIsOver()
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeapon() => weaponVisualController.SwitchOnCurrentWeaponModel();
}
