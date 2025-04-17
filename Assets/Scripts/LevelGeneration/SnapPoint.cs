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
        GetComponent<BoxCollider>().enabled = false; // Disable the collider to prevent unwanted interactions
        GetComponent<MeshRenderer>().enabled = false;

    }
    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
