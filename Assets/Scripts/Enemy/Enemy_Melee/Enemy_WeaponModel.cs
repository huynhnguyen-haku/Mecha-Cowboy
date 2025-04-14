using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType;
    public AnimatorOverrideController overrideController;
    public Enemy_MeleeWeaponData weaponData;   

    [SerializeField] private GameObject[] trailEffects;

    [Header("Damage Attributes")]
    public Transform[] damagePoints;
    public float attackCheckRadius;

    [ContextMenu("Assign Damage Points")]
    private void GetDamagePoints()
    {
        damagePoints = new Transform[trailEffects.Length];
        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }

    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }

    private void OnDrawGizmos()
    {
        if (damagePoints.Length > 0)
        {
            foreach (Transform point in damagePoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(point.position, attackCheckRadius);
            }
        }
    }
}