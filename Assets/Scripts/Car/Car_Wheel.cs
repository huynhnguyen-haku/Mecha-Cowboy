using UnityEngine;

public enum AxelType { Front, Rear }

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxelType axleType;
    public WheelCollider cd { get; private set; }
    public GameObject model { get; private set; }

    private float defaultSideStiffness;

    private void Awake()
    {
        cd = GetComponent<WheelCollider>();
        model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaltStiffness(float newValue)
    {
        defaultSideStiffness = newValue;
        RestoreDefaultStiffness();
    }

    public void RestoreDefaultStiffness()
    {
        WheelFrictionCurve sidewaysFriction = cd.sidewaysFriction;
        sidewaysFriction.stiffness = defaultSideStiffness;
        cd.sidewaysFriction = sidewaysFriction;
    }
}
