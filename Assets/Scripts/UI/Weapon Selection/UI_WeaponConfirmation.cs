using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponConfirmation : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponPriceText;
    [SerializeField] private TextMeshProUGUI confirmText;

    [SerializeField] private TextMeshProUGUI playerMoney;

    private Weapon_Data currentWeaponData;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        playerMoney.text = $"Money: {gameManager.playerMoney} golds";
    }

    public void SetupConfirmationUI(Weapon_Data weaponData)
    {
        currentWeaponData = weaponData;

        weaponIcon.sprite = weaponData.weaponIcon;
        weaponNameText.text = weaponData.weaponName;
        weaponPriceText.text = $"Price: {weaponData.price} golds";
        confirmText.text = "Do you want to buy this weapon?";
    }

    public void ConfirmPurchase()
    {
        if (GameManager.instance.playerMoney >= currentWeaponData.price)
        {
            // Pay and unlock the weapon
            GameManager.instance.AddMoney(-currentWeaponData.price);
            currentWeaponData.isUnlocked = true;

            Debug.Log($"Weapon {currentWeaponData.weaponName} unlocked!");
            confirmText.text = "Weapon unlocked!";

            // Back to weapon selection UI
            UI.instance.SwitchTo(UI.instance.weaponSelection.gameObject);
        }
        else
        {
            // Do not have enough money
            confirmText.text = "Not enough money!";
        }
    }

    public void ReturnToWeaponSelection()
    {
        // Back to weapon selection UI
        UI.instance.SwitchTo(UI.instance.weaponSelection.gameObject);
    }
}
