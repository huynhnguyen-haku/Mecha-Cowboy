using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Mission/Timer Mission")]

public class Mission_Timer : Mission
{
    public float time;
    private float currentTime;

    public override void StartMission()
    {
        currentTime = time;
    }

    public override void UpdateMission()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0)
        {

        }
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

        string missionText = "Get to the airplane before the time run out.";
        string missionDetails = "Time Left: " + timeText;

        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }

}
