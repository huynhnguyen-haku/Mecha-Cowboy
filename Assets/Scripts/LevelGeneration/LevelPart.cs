using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection Check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionCheckParent;

    private void Start()
    {
        if (intersectionCheckColliders.Length <= 0)
        {
            intersectionCheckColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }
    }

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();

        foreach (var collider in intersectionCheckColliders)
        {
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer);

            foreach (var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>();
                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();
        AlignTo(entrancePoint, targetSnapPoint);
        SnapTo(entrancePoint, targetSnapPoint);
    }

    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        var offset = transform.position - ownSnapPoint.transform.position;
        var newPosition = targetSnapPoint.transform.position + offset;
        transform.position = newPosition;
    }

    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // Calculate the rotation offset between the level part's current rotation and its own snap point's rotation
        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        // Align the level part's rotation to the target snap point's rotation
        transform.rotation = targetSnapPoint.transform.rotation;

        // Rotate the level part by 180 degrees to face the correct direction
        transform.Rotate(0, 180, 0);

        // Apply the rotation offset to fine-tune the alignment
        transform.Rotate(0, -rotationOffset, 0);
    }

    public SnapPoint GetEntrancePoint()
    {
        return GetSnapPointOfType(SnapPointType.Enter);
    }

    public SnapPoint GetExitPoint()
    {
        return GetSnapPointOfType(SnapPointType.Exit);
    }

    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        // Collect all snap points of the specified type
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == pointType)
            {
                filteredSnapPoints.Add(snapPoint);
            }
        }

        // If there are matching snap points, choose one at random
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        // If no matching snap points are found, return null
        return null;
    }
}
