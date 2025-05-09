using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    [SerializeField] private NavMeshSurface navMeshSurface;

    // Level parts  
    [SerializeField] private List<Transform> levelParts; // Tất cả các LevelPart
    [SerializeField] private Transform lastLevelPart; // Phần cuối của màn chơi
    private List<Transform> currentLevelParts; // Các LevelPart được lọc theo MissionType
    private List<Transform> generatedLevelParts = new List<Transform>(); // Các LevelPart đã được tạo

    // Snap points  
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    // Generation  
    [Space]
    [SerializeField] private float generationCooldown;
    private bool generationOver = true;
    private float cooldownTimer;

    // Enemies  
    private List<Enemy> enemyList;

    // Pathfinding Indicator
    private PathfindingIndicator pathfindingIndicator; // Tham chiếu đến PathfindingIndicator

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;
        pathfindingIndicator = FindObjectOfType<PathfindingIndicator>(); // Tìm PathfindingIndicator trong scene
        if (pathfindingIndicator == null)
        {
            Debug.LogError("LevelGenerator: PathfindingIndicator not found in scene!");
        }
    }

    private void Update()
    {
        if (generationOver)
            return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart();
            }
            else if (!generationOver)
            {
                FinishGeneration();
            }
        }
    }

    [ContextMenu("Restart Generation")]
    public void InitializeGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;

        // Lọc danh sách LevelPart theo MissionType
        MissionType currentMissionType = MissionManager.instance.currentMission.GetMissionType();
        currentLevelParts = new List<Transform>();

        foreach (Transform part in levelParts)
        {
            LevelPart levelPartScript = part.GetComponent<LevelPart>();
            if (levelPartScript != null && levelPartScript.missionTypes.Contains(currentMissionType))
            {
                currentLevelParts.Add(part);
            }
        }

        // Debug để kiểm tra danh sách đã lọc
        Debug.Log($"Filtered LevelParts for MissionType {currentMissionType}: {currentLevelParts.Count} parts found.");

        ClearGeneratedLevelParts();
    }

    private void ClearGeneratedLevelParts()
    {
        foreach (Enemy enemy in enemyList)
            Destroy(enemy.gameObject);

        foreach (Transform part in generatedLevelParts)
            Destroy(part.gameObject);

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();

        navMeshSurface.BuildNavMesh();

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }

        // Tìm MissionComplete_Zone và gán cho PathfindingIndicator
        GameObject missionCompleteZone = GameObject.Find("MissionComplete_Zone");
        if (missionCompleteZone != null && pathfindingIndicator != null)
        {
            pathfindingIndicator.SetTarget(missionCompleteZone.transform);
        }
        else
        {
            Debug.LogWarning("LevelGenerator: Could not find MissionComplete_Zone or PathfindingIndicator is null!");
        }

        MissionManager.instance.StartMission();
    }

    [ContextMenu("Generate Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        if (generationOver)
        {
            newPart = Instantiate(lastLevelPart);
        }
        else
        {
            newPart = Instantiate(ChooseRandomPart());
        }

        if (newPart == null)
        {
            Debug.LogError("No LevelPart could be generated. Check your MissionType filtering.");
            return;
        }

        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
        {
            InitializeGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitPoint();
        enemyList.AddRange(levelPartScript.GetEnemies());
    }

    private Transform ChooseRandomPart()
    {
        if (currentLevelParts.Count == 0)
        {
            Debug.LogError("No LevelParts available to generate. Check your MissionType filtering.");
            return null;
        }

        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);

        return chosenPart;
    }

    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }

    public List<Enemy> GetEnemyList() => enemyList;
}