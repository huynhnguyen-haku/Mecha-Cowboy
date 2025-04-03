using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_MeleeWeaponType { OneHand, Throw }

public class Enemy_Visual : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private Texture[] colorTexures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderers;

    [Header("Weapon")]
    [SerializeField] private Enemy_WeaponModel[] weaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;
    public GameObject currentWeaponModel;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
    }

    public void SetupVisual()
    {
        SetupRandomColor();
        SetupRandomWeapon();
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
    }

}