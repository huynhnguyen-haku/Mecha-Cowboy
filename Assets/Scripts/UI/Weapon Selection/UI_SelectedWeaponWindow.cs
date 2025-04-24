using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

    private void Start()
    {
        UpdateSlotInfo(null);
    }

    public void SetWeaponSlot(Weapon_Data newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(weaponData);
    }

    public void UpdateSlotInfo(Weapon_Data weapon_data)
    {
        if (weapon_data == null)
        {
            weaponIcon.color = Color.clear;
            weaponInfo.text = "No Weapon Selected";
            return;
        }

        weaponIcon.color = Color.white;
        weaponIcon.sprite = weapon_data.weaponIcon;
        weaponInfo.text = weapon_data.weaponInfo;
    }

    public bool IsEmpty()
    {
        return weaponData == null;
    }
}
