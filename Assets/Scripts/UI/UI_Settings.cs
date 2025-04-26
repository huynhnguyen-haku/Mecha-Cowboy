using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;

    [Header("Music Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderText;

    public void SfxSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void MusicSliderValue(float value)
    {
        musicSliderText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SetFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;
    }
}
