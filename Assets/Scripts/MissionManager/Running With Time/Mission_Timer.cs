using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Mission/Timer Mission")]
public class Mission_Timer : Mission
{
    public float time;
    private float currentTime;
    private bool isCompleted; // Theo dõi trạng thái hoàn thành

    private void OnEnable()
    {
        currentTime = time; // Reset thời gian
        isCompleted = false; // Reset trạng thái hoàn thành
    }

    public override void StartMission()
    {
        currentTime = time; // Đảm bảo thời gian khởi đầu đúng
        isCompleted = false;
        string missionText = "Get to the airplane before the time runs out.";
        string missionDetails = "Time Left: " + System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override void UpdateMission()
    {
        if (isCompleted)
            return; // Không cập nhật nếu đã thất bại hoặc hoàn thành

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

            string missionText = "Get to the airplane before the time runs out.";
            string missionDetails = "Time Left: " + timeText;
            UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
        }
        else
        {
            GameManager.instance.GameOver(); // Kết thúc game khi thất bại
        }
    }

    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }

    public override MissionType GetMissionType()
    {
        return MissionType.Default;
    }
}