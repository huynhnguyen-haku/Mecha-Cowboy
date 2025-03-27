using UnityEngine;

public class Weapon_PickUp : Interactable
{
    private WeaponController weaponController;
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
        UpdateGameObject();
    }

    public void SetUpPickupWeapon(Weapon weapon, Transform transform)
    {
        oldWeapon = true;

        this.weapon = weapon;
        weaponData = weapon.weaponData;

        this.transform.position = transform.position + transform.forward;
    }

    [ContextMenu("Update Weapon Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.weaponName.ToString();
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
        weaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnToPool(gameObject);
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
