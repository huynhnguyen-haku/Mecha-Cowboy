using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Last Defence Mission", menuName = "Mission/Last Defence - Mission")]
public class Mission_LastDefense : Mission
{
    public bool isDefenceStarted = false;
    public bool isMissionCompleted = false;

    [Header("Mission Details")]
    public float defenseDuration = 120;
    public float timeBetweenWaves = 15;

    private float defenseTimer;
    private float waveTimer;

    [Header("Respawn Details")]
    public int numberOfRespawnPoints = 2;
    public List<Transform> respawnPoints;
    private Vector3 defensePoint;

    [Space]
    public int numberOfEnemiesPerWave;
    public GameObject[] enemyPrefabs;
    private string defenceTimerText;

    private void OnEnable()
    {
        isDefenceStarted = false;
        isMissionCompleted = false;
    }

    public override void StartMission()
    {
        reward = 150;

        if (isMissionCompleted)
            return;

        // Tìm MissionObject_BaseToDefend và gán làm target cho PathfindingIndicator
        MissionObject_BaseToDefend baseToDefend = FindObjectOfType<MissionObject_BaseToDefend>();
        if (baseToDefend != null)
        {
            defensePoint = baseToDefend.transform.position;
            PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
            if (pathfindingIndicator != null)
            {
                pathfindingIndicator.SetTarget(baseToDefend.transform);
            }
        }
        else
        {
            Debug.LogWarning("Mission_LastDefense: MissionObject_BaseToDefend not found in scene!");
        }

        respawnPoints = new List<Transform>(ClosestPoints(numberOfRespawnPoints));

        string missionText = "Head to the maicious code zone to active it.";
        string missionDetails = "Tips: There are ammo boxes and powerful guns in nearby abandoned town.";
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override bool MissionCompleted()
    {
        return isMissionCompleted;
    }

    public override void UpdateMission()
    {
        if (isDefenceStarted == false || isMissionCompleted) return;

        waveTimer -= Time.deltaTime;

        if (defenseTimer > 0)
        {
            defenseTimer -= Time.deltaTime;
        }

        if (defenseTimer <= 0)
        {
            EndDefenseEvent();
            return;
        }

        if (waveTimer < 0)
        {
            CreateNewEnemies(numberOfEnemiesPerWave);
            waveTimer = timeBetweenWaves;
        }

        defenceTimerText = System.TimeSpan.FromSeconds(defenseTimer).ToString("mm':'ss");

        string missionText = "Reaced the malicious code zone. Activating...";
        string missionDetails = "Time Left: " + defenceTimerText;
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public void StartDefenseEvent()
    {
        waveTimer = 0.5f;
        defenseTimer = defenseDuration;
        isDefenceStarted = true;
    }

    private void EndDefenseEvent()
    {
        isDefenceStarted = false;
        HealthController.muteDeathSound = true;

        // Destroy all enemies in the scene
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in allEnemies)
        {
            HealthController healthController = enemy.GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.SetHealthToZero();
            }
        }
        isMissionCompleted = true;
        HealthController.muteDeathSound = false;

        string missionText = "The malicious code has been successfully activated. All enemies have been eliminated.";
        string missionDetails = "Leave the area by aircraft and claim the well-earned rewards awaiting you!";
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    private void CreateNewEnemies(int number)
    {
        // Kiểm tra nếu số lượng enemy và số lượng spawn points không khớp
        if (number != respawnPoints.Count)
        {
            return;
        }

        // Lặp qua từng spawn point và tạo enemy tương ứng
        for (int i = 0; i < respawnPoints.Count; i++)
        {
            Transform spawnPoint = respawnPoints[i];
            GameObject enemyPrefab = enemyPrefabs[i % enemyPrefabs.Length]; // Lấy enemy theo thứ tự, lặp lại nếu cần

            // Spawn enemy tại spawn point
            GameObject spawnedEnemy = ObjectPool.instance.GetObject(enemyPrefab, spawnPoint);
            Enemy enemyComponent = spawnedEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.arrgresssionRange = 100; // Thiết lập aggression range
            }
        }
    }


    private List<Transform> ClosestPoints(int number)
    {
        List<Transform> closestPoints = new List<Transform>();
        List<MissionObject_EnemyRespawnPoint> allPoints =
            new List<MissionObject_EnemyRespawnPoint>(FindObjectsOfType<MissionObject_EnemyRespawnPoint>());

        while (closestPoints.Count < number && allPoints.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            MissionObject_EnemyRespawnPoint closestPoint = null;

            foreach (var point in allPoints)
            {
                float distance = Vector3.Distance(defensePoint, point.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPoint = point;
                }
            }

            if (closestPoint != null)
            {
                closestPoints.Add(closestPoint.transform);
                allPoints.Remove(closestPoint);
            }
        }

        return closestPoints;
    }

    public override MissionType GetMissionType()
    {
        return MissionType.LastDefense;
    }
}