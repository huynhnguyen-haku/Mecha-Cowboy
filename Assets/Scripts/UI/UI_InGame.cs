using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image healthBar;

    [Header("Weapon Slots")]
    [SerializeField] private UI_WeaponSlot[] weaponSlots_UI;

    [Header("Mission")]
    [SerializeField] private GameObject missionUIParent;
    [SerializeField] private GameObject missionGuide;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionDetai;

    private bool MissionUIActive = true;

    private void Awake()
    {
        weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
    }
    
    public void ToggleMissionUI()
    {
        MissionUIActive = !MissionUIActive;
        missionUIParent.SetActive(MissionUIActive);
        missionGuide.SetActive(!MissionUIActive);
    }

    public void UpdateMissionUI(string mission, string missionDetails = "")
    {
        missionText.text = mission;
        missionDetai.text = missionDetails;
    }

    public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon currentWeapon)
    {
        for (int i = 0; i < weaponSlots_UI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                bool isACtiveWeapon = weaponSlots[i] == currentWeapon ? true : false; // Check if the weapon is the current active weapon
                weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isACtiveWeapon);
            }
            else
            {
                weaponSlots_UI[i].UpdateWeaponSlot(null, false);
            }
        }
    }

    public void UpdateHealthUI(float currenHealth, float maxHealth)
    {
        healthBar.fillAmount = currenHealth / maxHealth;
    }
}
