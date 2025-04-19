using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;


    // Level parts  
    [SerializeField] private List<Transform> levelParts;
    [SerializeField] private Transform lastLevelPart;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform>();

    // Snap points  
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    // Generation  
    [Space]
    [SerializeField] private float generationCooldown;
    private bool generationOver;
    private float cooldownTimer;

    // Enemies  
    private List<Enemy> enemyList;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;
        InitializeGeneration();
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
    private void InitializeGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);

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


        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null;
            enemy.gameObject.SetActive(true);
        }
    }

    [ContextMenu("Generate Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;
        if (generationOver)
            newPart = Instantiate(lastLevelPart);

        else
            newPart = Instantiate(ChooseRandomPart());

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
