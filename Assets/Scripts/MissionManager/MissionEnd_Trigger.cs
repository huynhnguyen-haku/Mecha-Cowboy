using UnityEngine;

public class MissionEnd_Trigger : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player) return;

        // Kiểm tra trạng thái nhiệm vụ trước khi hoàn thành
        if (MissionManager.instance.IsMissionCompleted())
        {
            Debug.Log("Mission already completed. No further action taken.");
            return;
        }

        if (MissionManager.instance.CompleteMission())
        {
            Debug.Log("Mission Completed");
        }
    }
}
