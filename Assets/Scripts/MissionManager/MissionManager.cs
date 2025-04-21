using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public Mission currentMission;

    public bool isMissionCompleted; // Cờ để lưu trạng thái nhiệm vụ

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Invoke(nameof(StartMission), 2);
    }

    private void Update()
    {
        currentMission?.UpdateMission();
    }

    private void StartMission()
    {
        currentMission.StartMission();
    }

    public bool CompleteMission()
    {
        if (isMissionCompleted)
        {
            Debug.Log("Mission already completed.");
            return false;
        }

        if (currentMission.MissionCompleted())
        {
            isMissionCompleted = true;
            return true;
        }

        Debug.Log("Mission not completed yet.");
        return false;
    }


    public bool IsMissionCompleted()
    {
        return isMissionCompleted;
    }
}
