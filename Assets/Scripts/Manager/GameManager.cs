using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("Settings")]
    public bool friendlyFire;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void SetDefaultWeapon()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }
}
