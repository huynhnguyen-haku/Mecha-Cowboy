using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Last Defence Mission", menuName = "Mission/Last Defence - Mission")]

public class Mission_LastDefense : Mission
{
    public bool isDefenceStarted = false;
    public bool isMissionCompleted = false; // New flag to track mission completion

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

    public override void StartMission()
    {
        if (isMissionCompleted)
            return; // Prevent starting the mission if it's already completed

        //defensePoint = FindObjectOfType<MissionEnd_Trigger>().transform.position;
        defensePoint = FindObjectOfType<MissionObject_BaseToDefend>().transform.position;
        respawnPoints = new List<Transform>(ClosestPoints(numberOfRespawnPoints));
    }

    public override bool MissionCompleted()
    {
        if (isMissionCompleted)
            return true; // Return true if the mission is already completed

        if (isDefenceStarted == false)
        {
            StartDefenceEvent();
            return false;
        }
        return false;
    }

    public override void UpdateMission()
    {
        if (isDefenceStarted == false || isMissionCompleted) return;

        defenseTimer -= Time.deltaTime;
        waveTimer -= Time.deltaTime;

        if (defenseTimer <= 0)
        {
            EndDefenceEvent();
            return;
        }

        if (waveTimer < 0)
        {
            CreateNewEnemies(numberOfEnemiesPerWave);
            waveTimer = timeBetweenWaves;
        }

        defenceTimerText = System.TimeSpan.FromSeconds(defenseTimer).ToString("mm':'ss");
        Debug.Log(defenceTimerText);
    }

    private void StartDefenceEvent()
    {
        waveTimer = 0.5f;
        defenseTimer = defenseDuration;
        isDefenceStarted = true;
    }

    private void EndDefenceEvent()
    {
        isDefenceStarted = false;

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
        isMissionCompleted = true; // Mark the mission as completed
        Debug.Log("Defense mission ended. All enemies have been defeated.");
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
