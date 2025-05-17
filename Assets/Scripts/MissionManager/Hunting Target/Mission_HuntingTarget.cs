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

        // Hủy đăng ký sự kiện trước khi đăng ký lại để tránh trùng lặp
        MissionObject_Target.OnTargetKilled -= ReduceRemainingTargets;
        MissionObject_Target.OnTargetKilled += ReduceRemainingTargets;

        List<Enemy> validEnemies = new List<Enemy>();

        // Tìm tất cả các enemy phù hợp với loại enemyType
        foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
        {
            if (enemy.enemyType == enemyType)
            {
                validEnemies.Add(enemy);
            }
        }

        // Gắn MissionObject_Target cho tất cả các enemy phù hợp
        foreach (Enemy enemy in validEnemies)
        {
            if (enemy.GetComponent<MissionObject_Target>() == null)
            {
                enemy.gameObject.AddComponent<MissionObject_Target>();
            }
        }

        // Debug để kiểm tra số lượng enemy được gắn script
        Debug.Log($"Total valid enemies with type {enemyType}: {validEnemies.Count}");
    }


    public override bool MissionCompleted()
    {
        return remainingTargets <= 0;
    }


    private void ReduceRemainingTargets()
    {
        remainingTargets--;
        UpdateMissionUI();

        // Kiểm tra nếu tất cả các target đã bị tiêu diệt
        if (remainingTargets <= 0)
        {
            // Cập nhật UI để thông báo hoàn thành nhiệm vụ

            string missionText = "Target eliminated.";
            string missionDetails = "Now go to the airplane to complete the mission.";
            UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);

            // Hủy đăng ký sự kiện để tránh lỗi
            MissionObject_Target.OnTargetKilled -= ReduceRemainingTargets;

            // Đánh dấu nhiệm vụ là hoàn thành
            Debug.Log("Mission completed!");
        }
    }

    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + numberOfTarget + " " + enemyType.ToString() + " enemies";
        string missionDetails = "Remaining: " + remainingTargets;

        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override MissionType GetMissionType()
    {
        return MissionType.HuntingTarget;
    }
}

