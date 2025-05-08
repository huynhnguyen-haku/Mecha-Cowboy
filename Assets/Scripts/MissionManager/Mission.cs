using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public string missionName;

    [TextArea]
    public string missionDescription;

    public int reward;

    [Header("Mission Preview")]
    public Sprite missionPreview;

    public abstract void StartMission();
    public abstract bool MissionCompleted();
    public virtual void UpdateMission() { }
    public abstract MissionType GetMissionType();


}
