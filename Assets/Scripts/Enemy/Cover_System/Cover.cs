using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("Cover Points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffSet = 1;
    [SerializeField] private float yOffSet = 0.2f;
    [SerializeField] private float zOffSet = 1;

    private void Start()
    {
        GenerateCoverPoints();
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

    public List<CoverPoint> GetCoverPoints()
    {
        return coverPoints;
    }
}
