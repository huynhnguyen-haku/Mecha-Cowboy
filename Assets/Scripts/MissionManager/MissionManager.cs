using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public Mission currentMission;
    private bool isMissionActive = false;

    private bool hasSetFinalTarget = false; // Flag để tránh gán target nhiều lần

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isMissionActive)
            currentMission?.UpdateMission();

        if (currentMission != null && currentMission.MissionCompleted() && !hasSetFinalTarget)
        {
            GameObject missionCompleteZone = GameObject.Find("MissionComplete_Zone");
            PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
            if (missionCompleteZone != null && pathfindingIndicator != null)
            {
                pathfindingIndicator.SetTarget(missionCompleteZone.transform);
                hasSetFinalTarget = true;
            }
        }

    }

    public void SetCurrentMission(Mission newMission)
    {
        currentMission = newMission;
        hasSetFinalTarget = false; // Reset flag khi bắt đầu mission mới
    }

    public void StartMission()
    {
        isMissionActive = true;
        currentMission.StartMission();
    }

    public bool MissionCompleted()
    {
        if (currentMission != null && currentMission.MissionCompleted())
        {
            GameManager.instance.AddMoney(currentMission.reward); // Cộng tiền thưởng
            Debug.Log($"Mission '{currentMission.missionName}' completed! Reward: {currentMission.reward} golds.");
            return true;
        }
        return false;
    }
}