using UnityEngine;

public enum AxelType { Front, Rear }

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxelType axleType;
    public WheelCollider cd { get; private set; }
    public GameObject model { get; private set; }

    private void Start()
    {
        cd = GetComponent<WheelCollider>();
        model = GetComponentInChildren<MeshRenderer>().gameObject;
    }
}
