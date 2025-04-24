using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }

    [SerializeField] private GameObject[] UIElements;


    private void Awake()
    {
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
    }

    public void SwitchTo(GameObject uiElementToActivate)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiElementToActivate.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        SwitchTo(inGameUI.gameObject);
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
}
