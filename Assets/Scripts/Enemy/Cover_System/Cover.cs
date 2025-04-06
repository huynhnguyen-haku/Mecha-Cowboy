using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Cover Points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffSet = 1;
    [SerializeField] private float yOffSet = 0.2f;
    [SerializeField] private float zOffSet = 1;

    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = GameObject.FindFirstObjectByType<Player>().transform;  
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3(0, yOffSet, -zOffSet),  // Front
            new Vector3(0, yOffSet, zOffSet),   // Back
            new Vector3(-xOffSet, yOffSet, 0),   // Right
            new Vector3(xOffSet, yOffSet, 0)    // Left
        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint);
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform).GetComponent<CoverPoint>();
            coverPoints.Add(coverPoint);
        }
    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemyTransform)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();
        foreach (CoverPoint coverPoint in coverPoints)
        {
            if (IsValidCoverPoint(coverPoint, enemyTransform))
            {
                validCoverPoints.Add(coverPoint);
            }
        }
        return validCoverPoints;
    }


    // Check if the cover point is valid
    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemyTransform)
    {
        // If the cover point is occupied by another enemy, it can't be used
        if (coverPoint.isOccupied)
        {
            return false;
        }

        // If the cover point is close to player, enemy can't take it
        if (IsCoverBehindPlayer(coverPoint, enemyTransform)) 
        {
            return false;
        }
        return true;
    }

    // Check if the cover point is behind the player
    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTransform)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);

        return distanceToPlayer < distanceToEnemy;
    }
}
