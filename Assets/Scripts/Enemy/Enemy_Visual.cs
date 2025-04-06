using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
public enum Enemy_RangeWeaponType { Pistol, Revolver, Shotgun, Rifle, Sniper }

public class Enemy_Visual : MonoBehaviour
{
    public GameObject currentWeaponModel;

    [Header("Color")]
    [SerializeField] private Texture[] colorTexures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderers;

    [Header("Crystal")]
    [SerializeField] private GameObject[] crystals;
    [SerializeField] private int crystalAmount;

    [Header("Rig Reference")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;


    public void EnableWeaponTrail(bool enable)
    {
        Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        currentWeaponScript.EnableTrailEffect(enable);

    }

    public void SetupVisual()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCrystals();
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTexures.Length);

        Material newMat = new Material(skinnedMeshRenderers.material);
        newMat.mainTexture = colorTexures[randomIndex];

        skinnedMeshRenderers.material = newMat;
    }
    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;


        if (thisEnemyIsMelee)
        {
            currentWeaponModel = FindMeleeWeaponModel();
        }

        if (thisEnemyIsRange)
        {
            currentWeaponModel = FindRangeWeaponModel();
        }

        currentWeaponModel.SetActive(true);
        OverrideAnimatorController();
    }
    private void SetupRandomCrystals()
    {
        List<int> availableIndexs = new List<int>();
        crystals = CollectCrystals();

        for (int i = 0; i < crystals.Length; i++)
        {
            availableIndexs.Add(i);
            crystals[i].SetActive(false);
        }

        for (int i = 0; i < crystalAmount; i++)
        {
            if (availableIndexs.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];

            crystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }



    private GameObject FindRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                SwitchAnimationLayer(((int)weaponModel.weaponHoldType));
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        Debug.LogError("No matching weapon model found for type: " + weaponType);
        return null;

    }
    private GameObject FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().weaponType;
        List<Enemy_WeaponModel> filteredWeaponModels = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                filteredWeaponModels.Add(weaponModel);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);
        return filteredWeaponModels[randomIndex].gameObject;
    }
    private GameObject[] CollectCrystals()
    {
        Enemy_Crystal[] crystalComponents = GetComponentsInChildren<Enemy_Crystal>(true);
        GameObject[] crystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            crystals[i] = crystalComponents[i].gameObject;
        }
        return crystals;
    }

    private void OverrideAnimatorController()
    {
        AnimatorOverrideController overrideController = currentWeaponModel.GetComponent<Enemy_WeaponModel>()?.overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }
    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator animator = GetComponentInChildren<Animator>();

        // Turn off all layers
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
        // Turn on the layer we want
        animator.SetLayerWeight(layerIndex, 1);
    }

    public void EnableIK(bool enableLeftHand, bool enableAim)
    {
        leftHandIKConstraint.weight = enableLeftHand ? 1 : 0;
        weaponAimConstraint.weight = enableAim ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {
        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;
    }
}