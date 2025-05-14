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

        // Chỉ bắt đầu defense nếu mission là LastDefense và chưa bắt đầu
        var mission = MissionManager.instance.currentMission as Mission_LastDefense;
        if (mission != null && !mission.isDefenceStarted)
        {
            mission.StartDefenseEvent();
            Debug.Log("MissionObject_BaseToDefend: Defense event started!");
        }
    }
}
