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

        PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
        if (pathfindingIndicator != null && currentMission != null && currentMission.MissionCompleted() && !hasSetFinalTarget)
        {
            GameObject missionCompleteZone = GameObject.Find("MissionComplete_Zone");
            if (missionCompleteZone != null)
            {
                pathfindingIndicator.SetTarget(missionCompleteZone.transform);
                hasSetFinalTarget = true;
                Debug.Log("MissionManager: Set PathfindingIndicator target to MissionComplete_Zone after mission completion.");
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

        // Play random mission BGM
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

    public void ResetAfterCompletion()
    {
        currentMission = null;
        isMissionActive = false;
        hasSetFinalTarget = false;
        Debug.Log("MissionManager: Reset after mission completion.");
    }
}