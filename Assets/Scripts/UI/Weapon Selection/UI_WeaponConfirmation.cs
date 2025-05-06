using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponConfirmation : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponPriceText;
    [SerializeField] private TextMeshProUGUI confirmText;

    private Weapon_Data currentWeaponData;

    public void SetupConfirmationUI(Weapon_Data weaponData)
    {
        currentWeaponData = weaponData;

        weaponIcon.sprite = weaponData.weaponIcon;
        weaponNameText.text = weaponData.weaponName;
        weaponPriceText.text = $"Price: {weaponData.price} coins";
        confirmText.text = "Do you want to buy this weapon?";
    }

    public void ConfirmPurchase()
    {
        if (GameManager.instance.playerMoney >= currentWeaponData.price)
        {
            // Tr? ti?n và m? khóa v? khí
            GameManager.instance.AddMoney(-currentWeaponData.price);
            currentWeaponData.isUnlocked = true;

            Debug.Log($"Weapon {currentWeaponData.weaponName} unlocked!");
            confirmText.text = "Weapon unlocked!";

            // Quay l?i UI Weapon Selection
            UI.instance.SwitchTo(UI.instance.weaponSelection.gameObject);
        }
        else
        {
            // Không ?? ti?n
            confirmText.text = "Not enough money!";
        }
    }

    public void ReturnToWeaponSelection()
    {
        // Quay l?i UI Weapon Selection
        UI.instance.SwitchTo(UI.instance.weaponSelection.gameObject);
    }
}
