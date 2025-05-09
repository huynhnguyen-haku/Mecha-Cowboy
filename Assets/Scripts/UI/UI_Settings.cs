using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
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

    [Header("Fire Settings")]
    [SerializeField] private Toggle friendlyFireToggle;
    [SerializeField] private Toggle preciseAimToggle;

    public void SfxSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMulti;
        audioMixer.SetFloat(sfxParameter, newValue);
    }

    public void MusicSliderValue(float value)
    {
        musicSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMulti;
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void SetFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;

        // Lưu ngay lập tức vào PlayerPrefs
        int friendlyFireValue = GameManager.instance.friendlyFire ? 1 : 0;
        PlayerPrefs.SetInt("FriendlyFire", friendlyFireValue);
        PlayerPrefs.Save(); // Đảm bảo lưu ngay lập tức
    }

    public void SetPreciseAimToggle()
    {
        bool isPreciseAim = Player_AimController.instance.isAimingPrecisly;
        Player_AimController.instance.isAimingPrecisly = !isPreciseAim;

        // Lưu ngay lập tức vào PlayerPrefs
        int preciseAimValue = Player_AimController.instance.isAimingPrecisly ? 1 : 0;
        PlayerPrefs.SetInt("PreciseAim", preciseAimValue);
        PlayerPrefs.Save();
    }


    public void LoadSettingsValues()
    {
        // Kiểm tra nếu giá trị đã tồn tại trong PlayerPrefs
        if (PlayerPrefs.HasKey("FriendlyFire"))
        {
            int friendlyFireValue = PlayerPrefs.GetInt("FriendlyFire");
            friendlyFireToggle.isOn = friendlyFireValue == 1;
        }
        else
        {
            // Thiết lập giá trị mặc định nếu không tồn tại
            friendlyFireToggle.isOn = false;
            GameManager.instance.friendlyFire = false;
        }

        // Tải giá trị âm thanh
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.5f); // Giá trị mặc định là 0.5
        musicSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.5f); // Giá trị mặc định là 0.5
    }
    private void OnDisable()
    {
        // Lưu Friendly Fire
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireValue = friendlyFire ? 1 : 0;
        PlayerPrefs.SetInt("FriendlyFire", friendlyFireValue);

        // Lưu âm thanh
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, musicSlider.value);

        // Đảm bảo lưu ngay lập tức
        PlayerPrefs.Save();
    }

}
