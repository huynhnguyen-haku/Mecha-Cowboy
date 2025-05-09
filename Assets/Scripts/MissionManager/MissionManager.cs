using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public Mission currentMission;
    private bool hasSetFinalTarget = false; // Flag để tránh gán target nhiều lần

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        currentMission?.UpdateMission();

        // Kiểm tra nếu mission là LastDefense và đã hoàn thành
        if (currentMission != null && currentMission.GetMissionType() == MissionType.LastDefense && !hasSetFinalTarget)
        {
            Mission_LastDefense lastDefenseMission = currentMission as Mission_LastDefense;
            if (lastDefenseMission != null && lastDefenseMission.isMissionCompleted)
            {
                // Tìm MissionComplete_Zone và gán làm target
                GameObject missionCompleteZone = GameObject.Find("MissionComplete_Zone");
                PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
                if (missionCompleteZone != null && pathfindingIndicator != null)
                {
                    pathfindingIndicator.SetTarget(missionCompleteZone.transform);
                    Debug.Log("MissionManager: LastDefense mission completed. Set PathfindingIndicator target to MissionComplete_Zone.");
                    hasSetFinalTarget = true; // Đánh dấu để không gán lại
                }
                else
                {
                    Debug.LogWarning("MissionManager: Could not find MissionComplete_Zone or PathfindingIndicator is null!");
                }
            }
        }
    }

    public void SetCurrentMission(Mission newMission)
    {
        currentMission = newMission;
        hasSetFinalTarget = false; // Reset flag khi bắt đầu mission mới
    }

    public void StartMission() => currentMission.StartMission();

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