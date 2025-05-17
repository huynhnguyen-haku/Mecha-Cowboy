using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Các danh sách BGM
    [SerializeField] private AudioSource mainMenuBGM; // Track cố định cho main menu
    [SerializeField] private AudioSource[] missionBGMs; // Danh sách track phát ngẫu nhiên trong mission
    [SerializeField] private AudioSource gameOverBGM; // Track cho game over
    [SerializeField] private AudioSource missionCompleteBGM; // Track cho mission complete

    [SerializeField] private bool[] loopTracks; // Xác định track nào sẽ lặp lại (dành cho missionBGMs)
    [SerializeField] private bool playBgm;
    [SerializeField] private float fadeDuration = 1f; // Thời gian fade mặc định (có thể điều chỉnh trong Inspector)

    private int currentMissionBGMIndex; // Theo dõi index của track mission hiện tại

    private void Awake()
    {
        instance = this;

        // Khởi tạo trạng thái ban đầu và kiểm tra lỗi
        if (mainMenuBGM == null || missionBGMs == null || gameOverBGM == null || missionCompleteBGM == null)
        {
            Debug.LogWarning("AudioManager: One or more BGM sources are not assigned!");
        }

        // Kiểm tra chi tiết mainMenuBGM
        if (mainMenuBGM != null)
        {
            if (mainMenuBGM.clip == null)
            {
                Debug.LogWarning("AudioManager: MainMenuBGM AudioSource is assigned, but no audio clip is set!");
            }
            else
            {
                Debug.Log($"AudioManager: MainMenuBGM is assigned with clip {mainMenuBGM.clip.name}, volume: {mainMenuBGM.volume}, playOnAwake: {mainMenuBGM.playOnAwake}");
            }
        }

        // Đảm bảo loopTracks có cùng độ dài với missionBGMs
        if (loopTracks == null || loopTracks.Length != missionBGMs.Length)
        {
            loopTracks = new bool[missionBGMs.Length];
        }
    }

    private void Start()
    {
        // Thử phát thủ công mainMenuBGM để kiểm tra
        if (mainMenuBGM != null && mainMenuBGM.clip != null)
        {
            Debug.Log("AudioManager: Attempting to play MainMenuBGM manually in Start...");
            mainMenuBGM.Play();
        }
    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = 0.85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        float pitch = Random.Range(minPitch, maxPitch);
        sfx.pitch = pitch;
        sfx.Play();
    }

    public void ControlSFX_FadeAndDelay(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        if (source == null)
            return;
        StartCoroutine(ProcessSFX_FadeAndDelay(source, play, targetVolume, delay, fadeDuration));
    }

    // Phát BGM chung với crossfade
    public void PlayBGM(int bgmType, float fadeDuration = 0f)
    {
        AudioSource newSource = null;
        string bgmTypeName = "";
        switch (bgmType)
        {
            case 0: // Main Menu
                if (mainMenuBGM == null)
                {
                    Debug.LogWarning("AudioManager: Main menu BGM is not assigned!");
                    return;
                }
                newSource = mainMenuBGM;
                bgmTypeName = "Main Menu";
                break;
            case 1: // Mission (random)
                if (missionBGMs == null || missionBGMs.Length == 0)
                {
                    Debug.LogWarning("AudioManager: No mission BGM tracks assigned!");
                    return;
                }
                int newIndex = Random.Range(0, missionBGMs.Length);
                while (newIndex == currentMissionBGMIndex && missionBGMs.Length > 1)
                {
                    newIndex = Random.Range(0, missionBGMs.Length);
                }
                currentMissionBGMIndex = newIndex;
                newSource = missionBGMs[currentMissionBGMIndex];
                bgmTypeName = "Mission";
                break;
            case 2: // Game Over
                if (gameOverBGM == null)
                {
                    Debug.LogWarning("AudioManager: Game over BGM is not assigned!");
                    return;
                }
                newSource = gameOverBGM;
                bgmTypeName = "Game Over";
                break;
            case 3: // Mission Complete
                if (missionCompleteBGM == null)
                {
                    Debug.LogWarning("AudioManager: Mission complete BGM is not assigned!");
                    return;
                }
                newSource = missionCompleteBGM;
                bgmTypeName = "Mission Complete";
                break;
            default:
                Debug.LogWarning("AudioManager: Invalid BGM type!");
                return;
        }

        Debug.Log($"AudioManager: Playing BGM type {bgmTypeName}");
        StartCoroutine(CrossfadeBGM(newSource, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
    }

    public void StopAllBGM(float fadeDuration = 0f)
    {
        AudioSource currentSource = GetCurrentPlayingBGM();
        if (currentSource != null)
        {
            Debug.Log($"AudioManager: Stopping BGM {currentSource.name} with fade duration {fadeDuration}");
            StartCoroutine(FadeOutBGM(currentSource, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
        }
    }

    private AudioSource GetCurrentPlayingBGM()
    {
        if (mainMenuBGM != null && mainMenuBGM.isPlaying) return mainMenuBGM;
        for (int i = 0; i < missionBGMs.Length; i++)
        {
            if (missionBGMs[i] != null && missionBGMs[i].isPlaying) return missionBGMs[i];
        }
        if (gameOverBGM != null && gameOverBGM.isPlaying) return gameOverBGM;
        if (missionCompleteBGM != null && missionCompleteBGM.isPlaying) return missionCompleteBGM;
        return null;
    }

    private void StopAllTracksExcept(AudioSource excludeSource, float fadeDuration)
    {
        if (mainMenuBGM != null && mainMenuBGM != excludeSource && mainMenuBGM.isPlaying)
        {
            Debug.Log($"AudioManager: Fading out MainMenuBGM with duration {fadeDuration}");
            StartCoroutine(FadeOutBGM(mainMenuBGM, fadeDuration));
        }

        for (int i = 0; i < missionBGMs.Length; i++)
        {
            if (missionBGMs[i] != null && missionBGMs[i] != excludeSource && missionBGMs[i].isPlaying)
            {
                Debug.Log($"AudioManager: Fading out MissionBGM[{i}] with duration {fadeDuration}");
                StartCoroutine(FadeOutBGM(missionBGMs[i], fadeDuration));
            }
        }

        if (gameOverBGM != null && gameOverBGM != excludeSource && gameOverBGM.isPlaying)
        {
            Debug.Log($"AudioManager: Fading out GameOverBGM with duration {fadeDuration}");
            StartCoroutine(FadeOutBGM(gameOverBGM, fadeDuration));
        }

        if (missionCompleteBGM != null && missionCompleteBGM != excludeSource && missionCompleteBGM.isPlaying)
        {
            Debug.Log($"AudioManager: Fading out MissionCompleteBGM with duration {fadeDuration}");
            StartCoroutine(FadeOutBGM(missionCompleteBGM, fadeDuration));
        }
    }

    private IEnumerator CrossfadeBGM(AudioSource newSource, float fadeDuration)
    {
        if (newSource == null)
        {
            Debug.LogWarning("AudioManager: CrossfadeBGM received a null AudioSource!");
            yield break;
        }

        // Lấy track hiện tại đang phát
        AudioSource oldSource = GetCurrentPlayingBGM();

        // Fade out tất cả các track khác ngoại trừ track mới
        StopAllTracksExcept(newSource, fadeDuration);

        // Cài đặt và phát track mới với fade-in
        newSource.volume = 0;
        newSource.loop = (newSource == missionBGMs[currentMissionBGMIndex]) ? loopTracks[currentMissionBGMIndex] : false;
        Debug.Log($"AudioManager: Starting playback for {newSource.name}, loop: {newSource.loop}");
        newSource.Play();

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // Fade-in track mới
            newSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        newSource.volume = 1;
        Debug.Log($"AudioManager: Finished fading in {newSource.name}, volume: {newSource.volume}");
    }

    private IEnumerator FadeOutBGM(AudioSource source, float fadeDuration)
    {
        if (source == null)
        {
            Debug.LogWarning("AudioManager: FadeOutBGM received a null AudioSource!");
            yield break;
        }

        float startVolume = source.volume;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset volume
        Debug.Log($"AudioManager: Finished fading out {source.name}, stopped and reset volume to {source.volume}");
    }

    private IEnumerator ProcessSFX_FadeAndDelay(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = play ? 0 : source.volume;
        float endVolume = play ? targetVolume : 0;
        float elapsedTime = 0;

        if (play)
        {
            source.volume = 0;
            source.Play();
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        source.volume = endVolume;

        if (play == false)
        {
            source.Stop();
        }
    }
}