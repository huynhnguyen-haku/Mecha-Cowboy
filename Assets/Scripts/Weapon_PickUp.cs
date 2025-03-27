using UnityEngine;

public class Weapon_PickUp : Interactable
{
    private WeaponController weaponController;
    [SerializeField] private Weapon_Data weaponData;

    [SerializeField] private BackupWeaponModel[] weaponModels;

    private void Start()
    {
        UpdateGameObject();
    }

    [ContextMenu("Update Weapon Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "Pickup " + weaponData.weaponName.ToString();
        UpdateWeaponModel();
    }

    public void UpdateWeaponModel()
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
        weaponController.PickupWeapon(weaponData);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
        {
            weaponController = other.GetComponent<WeaponController>();
        }
    }
}
