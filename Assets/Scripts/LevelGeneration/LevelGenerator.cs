using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> levelParts;
    [SerializeField] private Transform lastLevelPart;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform>();

    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private bool generationOver;
    private float cooldownTimer;

    private void Start()
    {
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
        foreach (Transform part in generatedLevelParts)
        {
            Destroy(part.gameObject);
        }

        generatedLevelParts = new List<Transform>();
    }

    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart();
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
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);

        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);

        return chosenPart;
    }
}
