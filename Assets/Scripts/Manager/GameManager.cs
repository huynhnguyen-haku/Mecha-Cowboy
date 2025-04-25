using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;

    [Header("Settings")]
    public bool friendlyFire;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
    }

    public void GameStart()
    {
        SetDefaultWeapon();
        LevelGenerator.instance.InitializeGeneration();

        // We start selected mission in a LevelGenerator script, after we done with level creation
    }

    public void RestartScene()
    {
        // Restart the scene from the main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(2);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(5);
    }

    public void CompleteGame()
    {
        UI.instance.DisplayVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable(); // Prevent player from moving
        player.health.currentHealth += 999; // Set player health to max just in case
    }

    private void SetDefaultWeapon()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }
}
