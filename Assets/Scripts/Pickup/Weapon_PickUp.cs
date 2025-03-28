using UnityEngine;

public class Weapon_PickUp : Interactable
{
    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Weapon weapon;

    [SerializeField] private BackupWeaponModel[] weaponModels;
    private bool oldWeapon;

    private void Start()
    {
        if (oldWeapon == false)
        {
            weapon = new Weapon(weaponData);
        }
        SetupGameObject();
    }

    public void SetUpPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + transform.forward;
    }

    [ContextMenu("Update Weapon Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponName.ToString();
        SetupWeaponModel();
    }

    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in weaponModels)
        {
            model.gameObject.SetActive(false);

            if (model.weaponType == weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponentInChildren<MeshRenderer>());
            }
        }
    }

    public override void Interact()
    {
        Debug.Log("Picked up weapon: " + weaponData.weaponName);
        weaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnToPool(gameObject);
    }
}
