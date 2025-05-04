using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderMulti = 25;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;
    [SerializeField] private string sfxParameter;

    [Header("Music Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderText;
    [SerializeField] private string bgmParameter;

    private void Start()
    {
        // Load saved values from PlayerPrefs
        LoadPauseMenuValues();
    }

    public void SfxSliderValue(float value)
    {
        // Update slider text
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";

        // Convert slider value to logarithmic scale and set it in the AudioMixer
        float newValue = Mathf.Log10(value) * sliderMulti;
        audioMixer.SetFloat(sfxParameter, newValue);

        // Save the value to PlayerPrefs
        PlayerPrefs.SetFloat(sfxParameter, value);
        PlayerPrefs.Save();
    }

    public void MusicSliderValue(float value)
    {
        // Update slider text
        musicSliderText.text = Mathf.RoundToInt(value * 100) + "%";

        // Convert slider value to logarithmic scale and set it in the AudioMixer
        float newValue = Mathf.Log10(value) * sliderMulti;
        audioMixer.SetFloat(bgmParameter, newValue);

        // Save the value to PlayerPrefs
        PlayerPrefs.SetFloat(bgmParameter, value);
        PlayerPrefs.Save();
    }

    private void LoadPauseMenuValues()
    {
        // Load SFX value
        float sfxValue = PlayerPrefs.GetFloat(sfxParameter, 0.5f); // Default value is 0.5
        sfxSlider.value = sfxValue;
        sfxSliderText.text = Mathf.RoundToInt(sfxValue * 100) + "%";

        // Load Music value
        float musicValue = PlayerPrefs.GetFloat(bgmParameter, 0.5f); // Default value is 0.5
        musicSlider.value = musicValue;
        musicSliderText.text = Mathf.RoundToInt(musicValue * 100) + "%";
    }
}
