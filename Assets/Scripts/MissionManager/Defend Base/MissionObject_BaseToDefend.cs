using UnityEngine;

public class MissionObject_BaseToDefend : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player)
            return;

        if (MissionManager.instance.MissionCompleted())
        {
            Debug.Log("Mission Complete");
        }
    }
}
