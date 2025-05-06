using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public Mission currentMission;


    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        currentMission?.UpdateMission();
    }

    public void SetCurrentMission(Mission newMission)
    {
        currentMission = newMission;
    }

    public void StartMission() => currentMission.StartMission();

    public bool MissionCompleted()
    {
        if (currentMission != null && currentMission.MissionCompleted())
        {
            GameManager.instance.AddMoney(currentMission.reward); // Cộng tiền thưởng
            Debug.Log($"Mission '{currentMission.missionName}' completed! Reward: {currentMission.reward} coins.");
            return true;
        }
        return false;
    }

}
