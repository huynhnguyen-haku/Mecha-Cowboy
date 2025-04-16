using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> levelParts;
    [SerializeField] private Transform lastLevelPart;
    private List<Transform> currentLevelParts;
    [SerializeField] private SnapPoint nextSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private bool generationOver;
    private float cooldownTimer;

    private void Start()
    {
        currentLevelParts = new List<Transform>(levelParts);
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

    private void FinishGeneration()
    {
        generationOver = true;
        Transform levelPart = Instantiate(lastLevelPart);
        LevelPart levelPartScript = levelPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
        Debug.Log("Level generation completed.");
    }

    [ContextMenu("Generate Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = Instantiate(ChooseRandomPart());
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
        {
            Debug.LogWarning("Intersection detected!");
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
