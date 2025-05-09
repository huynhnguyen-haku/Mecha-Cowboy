using UnityEngine;

public class WeaponUnlockManager : MonoBehaviour
{
    public static WeaponUnlockManager instance;

    [SerializeField] private Weapon_Data[] allWeapons;

    private const string UnlockKeyPrefix = "WeaponUnlocked_";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadAllWeaponStates();
    }

    public void SaveWeaponState(Weapon_Data weapon)
    {
        PlayerPrefs.SetInt(UnlockKeyPrefix + weapon.weaponName, weapon.isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadWeaponState(Weapon_Data weapon)
    {
        if (PlayerPrefs.HasKey(UnlockKeyPrefix + weapon.weaponName))
        {
            weapon.isUnlocked = PlayerPrefs.GetInt(UnlockKeyPrefix + weapon.weaponName) == 1;
        }
    }

    private void LoadAllWeaponStates()
    {
        foreach (var weapon in allWeapons)
        {
            LoadWeaponState(weapon);
        }
    }

    public void ResetAllWeaponStates()
    {
        foreach (var weapon in allWeapons)
        {
            PlayerPrefs.DeleteKey(UnlockKeyPrefix + weapon.weaponName);
            weapon.isUnlocked = false;
        }
        PlayerPrefs.Save();
    }
}
