using System.Collections;
using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines;
    private BoxCollider zoneCollider;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();
        ActivateWall(false);
    }

    private void ActivateWall(bool activate)
    {
        foreach(var line in lines)
        {
            if (activate)
                line.Play();

            else
                line.Stop();
        }
        zoneCollider.isTrigger = !activate;
    }

    IEnumerator WallActivation()
    {
        ActivateWall(true);
        yield return new WaitForSeconds(1f);
        ActivateWall(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WallActivation());
        Debug.Log("We don't have any business in that area! Don't go near it!");
    }
}
