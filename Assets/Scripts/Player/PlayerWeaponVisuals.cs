using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private bool isGrabbingWeapon;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupModels;

    [Header("Left Hand IK Settings")]
    [SerializeField] private Transform leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private float leftHandIK_WeightIncrease;
    private bool shouldIncrease_LeftHandIK_Weight;

    [Header("Rig Settings")]
    [SerializeField] private float rig_WeightIncrease;
    private bool shouldIncrease_Rig_Weight;
    private Rig rig;


    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandWeight();
    }


    public void PlayReloadAnimation()
    {
        if (isGrabbingWeapon)
            return;

        animator.SetTrigger("Reload");
        ReduceRigWeight();
    }
    public void PlayWeaponEquipAnimation()
    {
        GrabType grabType = CurrentWeaponModel().grabType;

        leftHandIK.weight = 0;
        ReduceRigWeight();
        animator.SetFloat("WeaponGrabType", (float)grabType);
        animator.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeapon(true);
    }


    public void SetBusyGrabbingWeapon(bool busy)
    {
        isGrabbingWeapon = busy;
        animator.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }
    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = (int)CurrentWeaponModel().holdType;

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if(player.weapon.HasOneWeapon()== false)
            SwitchOnBackupWeaponModels();

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }
    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    public void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in backupModels)
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModels()
    {
        WeaponType weaponType = player.weapon.BackupWeapon().weaponType;
        foreach (BackupWeaponModel backupModel in backupModels)
        {
            if (backupModel.weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        // Turn off all layers
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        // Turn on the layer we want
        animator.SetLayerWeight(layerIndex, 1);
    }
    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;
        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }
        return weaponModel;
    }


    #region Animation Rigging Method
    private void AttachLeftHand()
    {
        Transform targetTranform = CurrentWeaponModel().holdPoint;
        leftHandIK_Target.localPosition = targetTranform.localPosition;
        leftHandIK_Target.localRotation = targetTranform.localRotation;
    }
    private void UpdateLeftHandWeight()
    {
        if (shouldIncrease_LeftHandIK_Weight)
        {
            leftHandIK.weight += leftHandIK_WeightIncrease * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                shouldIncrease_LeftHandIK_Weight = false;
            }
        }
    }
    private void UpdateRigWeight()
    {
        if (shouldIncrease_Rig_Weight)
        {
            rig.weight += rig_WeightIncrease * Time.deltaTime;
            if (rig.weight >= 1)
            {
                shouldIncrease_Rig_Weight = false;
            }
        }
    }
    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }
    public void MaximizeRigWeight() => shouldIncrease_Rig_Weight = true;
    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandIK_Weight = true;
    #endregion
}

