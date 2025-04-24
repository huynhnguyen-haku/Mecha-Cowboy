using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public GameObject pauseUI;

    [SerializeField] private GameObject[] UIElements;


    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
    }

    private void Start()
    {
        AssignInputUI();
    }

    public void SwitchTo(GameObject uiElementToActivate)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiElementToActivate.SetActive(true);
    }


    public void StartGame()
    {
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    public void RestartGame()
    {
        GameManager.instance.RestartScene();    
    }

    public void TogglePauseUI()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            Time.timeScale = 1;
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            Time.timeScale = 0;
        }
    }

    private void AssignInputUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;
        controls.UI.TogglePauseUI.performed += ctx => TogglePauseUI();
    }
}
