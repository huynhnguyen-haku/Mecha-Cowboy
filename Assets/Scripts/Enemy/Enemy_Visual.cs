using System.Collections.Generic;
using UnityEngine;

public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed}

public class Enemy_Visual : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private Texture[] colorTexures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderers;

    [Header("Weapon")]
    [SerializeField] private Enemy_WeaponModel[] weaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;
    public GameObject currentWeaponModel;

    [Header("Crystal")]
    [SerializeField] private GameObject[] crystals;
    [SerializeField] private int crystalAmount;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        CollectCrystals();
    }


    public void SetupVisual()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCrystals();
    }

    private void SetupRandomCrystals()
    {
        List<int> availableIndexs = new List<int>();

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

    public void SetupWeaponType(Enemy_MeleeWeaponType type) => weaponType = type;

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTexures.Length);

        Material newMat = new Material(skinnedMeshRenderers.material);
        newMat.mainTexture = colorTexures[randomIndex];

        skinnedMeshRenderers.material = newMat;
    }

    private void SetupRandomWeapon()
    {
        foreach (var weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<Enemy_WeaponModel> filteredWeaponModels = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
                filteredWeaponModels.Add(weaponModel);
        }


        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        currentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
        currentWeaponModel.SetActive(true);

        OverrideAnimatorController();
    }

    private void OverrideAnimatorController()
    {
        AnimatorOverrideController overrideController = currentWeaponModel.GetComponent<Enemy_WeaponModel>().overrideController;
        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void CollectCrystals()
    {
        Enemy_Crystal[] crystalComponents = GetComponentsInChildren<Enemy_Crystal>(true);
        crystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            crystals[i] = crystalComponents[i].gameObject;
        }
    }
}