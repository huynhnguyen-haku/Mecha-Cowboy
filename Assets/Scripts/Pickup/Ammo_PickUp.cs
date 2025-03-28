using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    smallBox,
    bigBox
}

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}

public class Ammo_PickUp : Interactable
{
    [SerializeField] private AmmoBoxType boxType;
    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }


    public override void Interact()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.bigBox)
            currentAmmoList = bigBoxAmmo;

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);
            AddBullets(weapon, GetBulletAmount(ammo));
        }

        ObjectPool.instance.ReturnToPool(gameObject);
    }


    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        float randomAmmoAmount = Random.Range(min, max);

        // Round to the nearest whole number
        return Mathf.RoundToInt(randomAmmoAmount); 
    }

    private void AddBullets(Weapon weapon, int amount)
    {
        if (weapon == null)
            return;

        weapon.TotalReserveAmmo += amount;
    }


    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i == (int)boxType)
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponentInChildren<MeshRenderer>());
            }
        }
    }

}
