using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIElementToActivate;
    public UI_SelectedWeaponWindow[] selectedWeapon;

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float disappearingSpeed = 0.25f;

    private float currentWarningAlpha;
    private float targetWarningAlpha;

    private void Start()
    {
      selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disappearingSpeed;
            warningText.color = new Color (1, 1, 1, currentWarningAlpha);
        }
    }

    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> selectedData = new List<Weapon_Data>();

        foreach (UI_SelectedWeaponWindow weapon in selectedWeapon)
        {
            if (weapon.weaponData != null)
            {
                selectedData.Add(weapon.weaponData);
            }
        }
        return selectedData;
    }

    public void ConfirmWeaponSelection()
    {
        if (HasSelectedWeapon())
        {
            UI.instance.SwitchTo(nextUIElementToActivate);

            // Generate level part here so there won't be any issues with weapon selection
            UI.instance.StartLevelGeneration();
        }
        else
        {
            ShowWarningMessage("Please select at least one weapon.");
        }
    }

    private bool HasSelectedWeapon() => SelectedWeaponData().Count > 0;

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

    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white;
        warningText.text = message;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }
}
