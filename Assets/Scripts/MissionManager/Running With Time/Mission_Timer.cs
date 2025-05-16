using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Mission/Timer Mission")]
public class Mission_Timer : Mission
{
    public float time;
    private float currentTime;
    private bool hasFailed; // Theo dõi trạng thái thất bại
    private bool isCompleted; // Theo dõi trạng thái hoàn thành

    private void OnEnable()
    {
        currentTime = time; // Reset thời gian
        hasFailed = false; // Reset trạng thái thất bại
        isCompleted = false; // Reset trạng thái hoàn thành
        Debug.Log("Mission_Timer: Reset state with time " + time + " seconds.");
    }

    public override void StartMission()
    {
        currentTime = time; // Đảm bảo thời gian khởi đầu đúng
        hasFailed = false;
        isCompleted = false;
        string missionText = "Get to the airplane before the time runs out.";
        string missionDetails = "Time Left: " + System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override void UpdateMission()
    {
        if (hasFailed || isCompleted) return; // Không cập nhật nếu đã thất bại hoặc hoàn thành

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
            // Thời gian hết, mission thất bại
            hasFailed = true;
            currentTime = 0;
            string missionText = "Time's up!";
            string missionDetails = "Mission failed. Return to main menu to try again.";
            UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
            GameManager.instance.GameOver(); // Kết thúc game khi thất bại
        }
    }

    public override bool MissionCompleted()
    {
        return isCompleted && currentTime > 0 && !hasFailed; // Hoàn thành nếu được đánh dấu hoàn thành, thời gian còn, và chưa thất bại
    }

    // Được gọi bởi MissionEnd_Trigger khi người chơi đến đích
    public void MarkAsCompleted()
    {
        isCompleted = true;
        Debug.Log("Mission_Timer: Marked as completed.");
    }

    public override MissionType GetMissionType()
    {
        return MissionType.Default;
    }
}