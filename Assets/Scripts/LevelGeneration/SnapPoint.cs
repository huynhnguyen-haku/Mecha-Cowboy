using UnityEngine;

public enum SnapPointType
{
    Enter,
    Exit,
}

public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    private void Start()
    {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

    }
    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
