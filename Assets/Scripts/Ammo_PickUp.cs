using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    smallBox,
    bigBox
}

public class Ammo_PickUp : Interactable
{
    private WeaponController weaponController;

    [System.Serializable]
    public struct AmmoData
    {
        public WeaponType weaponType;
        public int amount;
    }

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
            AddBullets(weapon, ammo.amount);
        }

        ObjectPool.instance.ReturnToPool(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
            weaponController = other.GetComponent<WeaponController>();
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
