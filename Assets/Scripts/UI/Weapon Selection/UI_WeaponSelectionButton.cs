using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponSelectionButton : UI_Button
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Weapon_Data weaponData;

    private UI_WeaponSelection weaponSelectionUI;
    private UI_SelectedWeaponWindow emptySlot;
    
    private void OnValidate()
    {
        gameObject.name = "Button - Select Weapon: " + weaponData.weaponName;
    }

    public override void Start()
    {
        base.Start();
        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
        weaponIcon.sprite = weaponData.weaponIcon;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        emptySlot = weaponSelectionUI.FindEmptySlot();
        emptySlot?.UpdateSlotInfo(weaponData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        emptySlot?.UpdateSlotInfo(null);
        emptySlot = null;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
}
