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
            Debug.Log("Game over");
        }
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
        Debug.Log(timeText);
    }

    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }

}
