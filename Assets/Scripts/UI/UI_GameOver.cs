using TMPro;
using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;

    public void ShowGameOverMessage(string message)
    {
        gameOverText.text = message;
        gameObject.SetActive(true);
    }
}
