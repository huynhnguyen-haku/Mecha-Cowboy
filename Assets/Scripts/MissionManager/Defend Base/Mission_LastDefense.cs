using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Last Defence Mission", menuName = "Mission/Last Defence - Mission")]

public class Mission_LastDefense : Mission
{
    public bool isDefenceStarted;

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
    }

    public override bool MissionCompleted()
    {
        if (isDefenceStarted == false)
        {
            StartDefenceEvent();
            return true;
        }
        return defenseTimer <= 0;
    }

    public override void StartMission()
    {
        defensePoint = FindObjectOfType<MissionEnd_Trigger>().transform.position;
        respawnPoints = new List<Transform>(ClosestPoints(numberOfRespawnPoints));
    }

    public override void UpdateMission()
    {
        if (isDefenceStarted == false)
            return;

        defenseTimer -= Time.deltaTime;
        waveTimer -= Time.deltaTime;

        if (defenseTimer <= 0)
        {
            EndDefenceEvent(); // Gọi phương thức kết thúc nhiệm vụ
            return; // Dừng việc spawn enemy
        }

        if (waveTimer < 0)
        {
            CreateNewEnemies(numberOfEnemiesPerWave);
            waveTimer = timeBetweenWaves; // Cool down for the next wave
        }

        defenceTimerText = System.TimeSpan.FromSeconds(defenseTimer).ToString("mm':'ss");
        Debug.Log(defenceTimerText);
    }


    private void CreateNewEnemies(int number)
    {

        for (int i = 0; i < number; i++)
        {
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            int randomRespawnIndex = Random.Range(0, respawnPoints.Count);

            Transform randomRespawnPoint = respawnPoints[randomRespawnIndex];
            GameObject randomEnemy = enemyPrefabs[randomEnemyIndex];

            randomEnemy.GetComponent<Enemy>().arrgresssionRange = 100;
            ObjectPool.instance.GetObject(randomEnemy, randomRespawnPoint);
        }

        if (enemyPrefabs.Length == 0 || respawnPoints.Count == 0) return;
    }

    private void StartDefenceEvent()
    {
        waveTimer = 0.5f; // Start the first wave immediately
        defenseTimer = defenseDuration;
        isDefenceStarted = true;
    }

    private void EndDefenceEvent()
    {
        isDefenceStarted = false; // Dừng nhiệm vụ phòng thủ

        // Tìm tất cả các kẻ địch trong scene
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in allEnemies)
        {
            HealthController healthController = enemy.GetComponent<HealthController>();
            if (healthController != null)
            {
                healthController.SetHealthToZero();
            }
        }

        Debug.Log("Defense mission ended. All enemies have been defeated.");
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
}
