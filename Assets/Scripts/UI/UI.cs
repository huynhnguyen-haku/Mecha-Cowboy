using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public GameObject pauseUI;

    [SerializeField] private GameObject[] UIElements;

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage;


    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
    }

    private void Start()
    {
        AssignInputUI();
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));
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
        StartCoroutine(StartGameSequence());
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
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.instance.RestartScene));
    }

    public void ShowGameOverUI(string message = "Game Over")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

    public void TogglePauseUI()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
        }
    }

    private void AssignInputUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;
        controls.UI.TogglePauseUI.performed += ctx => TogglePauseUI();
    }

    private IEnumerator StartGameSequence()
    {
        StartCoroutine(ChangeImageAlpha(1, 1, null));
        yield return new WaitForSeconds(1f);
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
        StartCoroutine(ChangeImageAlpha(0, 1f, null));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float timeElapsed = 0f;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        onComplete?.Invoke();
    }
}
