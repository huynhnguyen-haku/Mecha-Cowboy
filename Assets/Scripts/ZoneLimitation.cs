using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines;
    private BoxCollider zoneCollider;

    private void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();
    }

    private void ActivateWall()
    {

    }
}
