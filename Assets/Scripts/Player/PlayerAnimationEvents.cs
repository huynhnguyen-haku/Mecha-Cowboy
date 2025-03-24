using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals weaponVisualController;
    private WeaponController weaponController;

    private void Start()
    {
        weaponVisualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<WeaponController>();
    }

    public void ReloadIsOver()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();
    }
    public void ReturnRig()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponVisualController.MaximizeLeftHandWeight();
    }

    public void GrabIsOver()
    {
        weaponVisualController.SetBusyGrabbingWeapon(false);
    }

    public void SwitchOnWeapon() => weaponVisualController.SwitchOnCurrentWeaponModel();
}
