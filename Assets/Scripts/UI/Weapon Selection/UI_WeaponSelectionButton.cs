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

        weaponIcon.color = Color.yellow;
        emptySlot = weaponSelectionUI.FindEmptySlot();
        emptySlot?.UpdateSlotInfo(weaponData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        weaponIcon.color = Color.white;
        emptySlot?.UpdateSlotInfo(null);
        emptySlot = null;
    }

    //public override void OnPointerDown(PointerEventData eventData)
    //{
    //    base.OnPointerDown(eventData);
    //    weaponIcon.color = Color.white;

    //    bool isNoEmptySlotAvailable = weaponSelectionUI.FindEmptySlot() == null;
    //    bool isWeaponTypeNotAssigned = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData) == null;

    //    if (isNoEmptySlotAvailable && isWeaponTypeNotAssigned)
    //    {
    //        weaponSelectionUI.ShowWarningMessage("No empty slot...");
    //        return;
    //    }

    //    UI_SelectedWeaponWindow assignedWeaponSlot = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData);
    //    if (assignedWeaponSlot != null)
    //    {
    //        assignedWeaponSlot.SetWeaponSlot(null);
    //    }
    //    else
    //    {
    //        emptySlot = weaponSelectionUI.FindEmptySlot();
    //        emptySlot?.SetWeaponSlot(weaponData);
    //    }

    //    emptySlot = null; // Important to reset the reference to avoid multiple updates
    //}

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        weaponIcon.color = Color.white;

        // Kiểm tra xem vũ khí đã được chọn trong slot nào chưa
        UI_SelectedWeaponWindow assignedWeaponSlot = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData);
        if (assignedWeaponSlot != null)
        {
            // Nếu đã được chọn, hủy chọn vũ khí khỏi slot
            assignedWeaponSlot.SetWeaponSlot(null);
            return;
        }

        // Nếu chưa được chọn, tìm slot trống để gán vũ khí
        emptySlot = weaponSelectionUI.FindEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.SetWeaponSlot(weaponData);
        }
        else
        {
            // Nếu không có slot trống, hiển thị thông báo
            weaponSelectionUI.ShowWarningMessage("No empty slot...");
        }

        emptySlot = null; // Reset lại tham chiếu
    }

}
