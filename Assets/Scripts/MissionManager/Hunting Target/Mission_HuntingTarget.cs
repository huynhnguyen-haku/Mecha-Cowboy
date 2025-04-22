using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Hunting Target", menuName = "Mission/Hunting Target")]
public class Mission_HuntingTarget : Mission
{
    public int numberOfTarget;
    private int remainingTargets;
    public EnemyType enemyType;

    public override void StartMission()
    {
        remainingTargets = numberOfTarget;
        UpdateMissionUI();
        MissionObject_Target.OnTargetKilled += ReduceRemainingTargets;

        List<Enemy> validEnemies = new List<Enemy>();

        foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
        {
            if (enemy.enemyType == enemyType)
            {
                validEnemies.Add(enemy);
            }
        }

        for (int i = 0; i < numberOfTarget; i++)
        {
            if (validEnemies.Count <= 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_Target>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    public override bool MissionCompleted()
    {
        return remainingTargets <= 0;
    }

    private void ReduceRemainingTargets()
    {
        remainingTargets--;
        UpdateMissionUI();

        if (remainingTargets <= 0)
        {
            UI.instance.inGameUI.UpdateMissionUI("Target eliminated. Get to the evacuation point to complete mission");
            MissionObject_Target.OnTargetKilled -= ReduceRemainingTargets;
        }
    }
    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + numberOfTarget + " " + enemyType.ToString() + " enemies";
        string missionDetails = "Remaining: " + remainingTargets;

        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }
}

