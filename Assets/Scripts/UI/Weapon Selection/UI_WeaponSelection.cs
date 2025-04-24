using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    public UI_SelectedWeaponWindow[] selectedWeapon;

    private void Start()
    {
      selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    // This method is used to find an empty slot in the selected weapon array.
    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].IsEmpty())
            {
                return selectedWeapon[i];
            }
        }

        return null;
    }

    // This method is used to find a slot that contains a weapon of a specific type.
    public UI_SelectedWeaponWindow FindSlotWithWeaponOfType(Weapon_Data weaponData)
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].weaponData == weaponData)
            {
                return selectedWeapon[i];
            }
        }

        return null;
    }
}
