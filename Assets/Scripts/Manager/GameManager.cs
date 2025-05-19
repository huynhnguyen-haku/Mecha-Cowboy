using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Car_Controller currentCar;

    public int playerMoney;

    [Header("Settings")]
    public bool friendlyFire;

    public bool quickStart; // Remove this in the final build



    // Trạng thái game để quản lý BGM
    private enum GameState { MainMenu, InGame, GameOver, MissionComplete }
    private GameState currentGameState = GameState.MainMenu;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();
        LoadPlayerMoney();
    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    // Testing areas
    public void AddMoney(int amount)
    {
        playerMoney += amount;
        Debug.Log($"Player received {amount} golds. Total money: {playerMoney}");

        // Lưu số tiền vào PlayerPrefs
        SavePlayerMoney();
    }

    private void SavePlayerMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", playerMoney);
        PlayerPrefs.Save(); // Đảm bảo lưu ngay lập tức
    }

    private void LoadPlayerMoney()
    {
        // Tải số tiền từ PlayerPrefs, mặc định là 0 nếu chưa có
        playerMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
        Debug.Log($"Loaded player money: {playerMoney}");
    }
    // End of testing areas

    public void GameStart()
    {
        SetDefaultWeapon();
        Cursor.visible = false;
        // Start selected mission in a LevelGenerator script, after we done with level creation
        UpdateGameState(GameState.InGame);
    }

    public void RestartScene()
    {
        // Restart the scene from the main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.visible = true;

        PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
        if (pathfindingIndicator != null)
        {
            pathfindingIndicator.Reset();
        }
        else
        {
            Debug.LogWarning("GameManager: PathfindingIndicator not found during RestartScene!");
        }

        UpdateGameState(GameState.MainMenu);
    }

    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(2);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(5);
        Cursor.visible = true;
        UpdateGameState(GameState.GameOver);
    }

    public void CompleteGame()
    {
        UI.instance.DisplayVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable(); // Prevent player from moving
        player.health.currentHealth += 999; // Set player health to max just in case
        Cursor.visible = true;
        UpdateGameState(GameState.MissionComplete);
    }

    private void SetDefaultWeapon()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }

    private void UpdateGameState(GameState newState)
    {
        if (currentGameState == newState)
            return;

        currentGameState = newState;

        // Phát BGM dựa trên trạng thái game
        switch (currentGameState)
        {
            case GameState.MainMenu:
                AudioManager.instance.PlayBGM(0); // Main Menu
                break;

            case GameState.InGame:
                AudioManager.instance.PlayBGM(1); // Mission
                break;

            case GameState.GameOver:
                AudioManager.instance.PlayBGM(2); // Game Over
                break;

            case GameState.MissionComplete:
                AudioManager.instance.PlayBGM(3); // Mission Complete
                break;
        }

        Debug.Log($"GameManager: Updated state to {currentGameState}");
    }
}