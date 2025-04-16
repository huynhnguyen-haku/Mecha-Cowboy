using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> levelParts;
    [SerializeField] private SnapPoint nextSnapPoint;

    private void Start()
    {
        GenerateNextLevelPart();
    }

    [ContextMenu("Generate Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = Instantiate(ChooseRandomPart());
        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();

        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);
        nextSnapPoint = levelPartScript.GetExitPoint();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, levelParts.Count);

        Transform chosenPart = levelParts[randomIndex];

        levelParts.RemoveAt(randomIndex);

        return chosenPart;
    }
}
