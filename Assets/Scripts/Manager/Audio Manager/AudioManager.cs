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
    private bool isTrackPlaying; // Theo dõi trạng thái phát
    private bool isFading; // Theo dõi trạng thái fade để tránh xung đột

    // Trạng thái hiện tại của BGM
    private enum BGMState { None, MainMenu, Mission, GameOver, MissionComplete }
    private BGMState currentBGMState = BGMState.None;

    private void Awake()
    {
        instance = this;

        // Khởi tạo trạng thái ban đầu và kiểm tra lỗi
        if (mainMenuBGM == null || missionBGMs == null || gameOverBGM == null || missionCompleteBGM == null)
        {
            Debug.LogWarning("AudioManager: One or more BGM sources are not assigned!");
        }

        // Đảm bảo loopTracks có cùng độ dài với missionBGMs
        if (loopTracks == null || loopTracks.Length != missionBGMs.Length)
        {
            loopTracks = new bool[missionBGMs.Length];
        }
    }

    private void Start()
    {
        PlayMainMenuBGM(); // Phát track main menu khi bắt đầu với fade-in
    }

    private void Update()
    {
        // Chỉ kiểm tra nếu playBgm bật và không đang fade
        if (playBgm && !isFading)
        {
            // Nếu đang ở trạng thái mission và track hiện tại kết thúc (không lặp), phát track mới
            if (currentBGMState == BGMState.Mission && isTrackPlaying && missionBGMs.Length > 0 && !missionBGMs[currentMissionBGMIndex].isPlaying && !loopTracks[currentMissionBGMIndex])
            {
                PlayRandomMissionBGM();
            }

            // Cập nhật trạng thái phát
            AudioSource currentSource = GetCurrentPlayingBGM();
            isTrackPlaying = (currentSource != null && currentSource.isPlaying);
        }

        // Nếu playBgm tắt, fade out BGM nếu đang phát
        if (!playBgm && isTrackPlaying && !isFading)
        {
            StopAllBGM(fadeDuration); // Fade out trong thời gian fadeDuration
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

    // Phát BGM cho main menu với fade-in
    public void PlayMainMenuBGM(float fadeDuration = 0f)
    {
        if (mainMenuBGM == null)
        {
            Debug.LogWarning("AudioManager: Main menu BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.MainMenu;
        StartCoroutine(PlayWithFadeIn(mainMenuBGM, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
    }

    // Phát BGM ngẫu nhiên cho mission với fade-in
    public void PlayRandomMissionBGM(float fadeDuration = 0f)
    {
        if (missionBGMs == null || missionBGMs.Length == 0)
        {
            Debug.LogWarning("AudioManager: No mission BGM tracks assigned!");
            return;
        }

        int newIndex = Random.Range(0, missionBGMs.Length);
        // Tránh phát lại track hiện tại nếu có thể
        while (newIndex == currentMissionBGMIndex && missionBGMs.Length > 1)
        {
            newIndex = Random.Range(0, missionBGMs.Length);
        }
        currentMissionBGMIndex = newIndex;

        currentBGMState = BGMState.Mission;
        StartCoroutine(PlayWithFadeIn(missionBGMs[currentMissionBGMIndex], fadeDuration > 0 ? fadeDuration : this.fadeDuration));
    }

    // Phát BGM khi game over với fade-in
    public void PlayGameOverBGM(float fadeDuration = 0f)
    {
        if (gameOverBGM == null)
        {
            Debug.LogWarning("AudioManager: Game over BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.GameOver;
        StartCoroutine(PlayWithFadeIn(gameOverBGM, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
    }

    // Phát BGM khi mission complete với fade-in
    public void PlayMissionCompleteBGM(float fadeDuration = 0f)
    {
        if (missionCompleteBGM == null)
        {
            Debug.LogWarning("AudioManager: Mission complete BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.MissionComplete;
        StartCoroutine(PlayWithFadeIn(missionCompleteBGM, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
    }

    public void StopAllBGM(float fadeDuration = 0f)
    {
        AudioSource currentSource = GetCurrentPlayingBGM();
        if (currentSource != null)
        {
            StartCoroutine(FadeOutBGM(currentSource, fadeDuration > 0 ? fadeDuration : this.fadeDuration));
        }
        currentBGMState = BGMState.None;
    }

    private AudioSource GetCurrentPlayingBGM()
    {
        if (currentBGMState == BGMState.MainMenu && mainMenuBGM != null && mainMenuBGM.isPlaying)
            return mainMenuBGM;

        if (currentBGMState == BGMState.Mission && missionBGMs.Length > 0 && missionBGMs[currentMissionBGMIndex].isPlaying)
            return missionBGMs[currentMissionBGMIndex];

        if (currentBGMState == BGMState.GameOver && gameOverBGM != null && gameOverBGM.isPlaying)
            return gameOverBGM;

        if (currentBGMState == BGMState.MissionComplete && missionCompleteBGM != null && missionCompleteBGM.isPlaying)
            return missionCompleteBGM;

        return null;
    }

    private void StopAllTracksExcept(AudioSource excludeSource)
    {
        if (mainMenuBGM != null && mainMenuBGM != excludeSource && mainMenuBGM.isPlaying)
        {
            mainMenuBGM.Stop();
            mainMenuBGM.volume = 1; // Reset volume
        }

        for (int i = 0; i < missionBGMs.Length; i++)
        {
            if (missionBGMs[i] != null && missionBGMs[i] != excludeSource && missionBGMs[i].isPlaying)
            {
                missionBGMs[i].Stop();
                missionBGMs[i].volume = 1; // Reset volume
            }
        }

        if (gameOverBGM != null && gameOverBGM != excludeSource && gameOverBGM.isPlaying)
        {
            gameOverBGM.Stop();
            gameOverBGM.volume = 1; // Reset volume
        }

        if (missionCompleteBGM != null && missionCompleteBGM != excludeSource && missionCompleteBGM.isPlaying)
        {
            missionCompleteBGM.Stop();
            missionCompleteBGM.volume = 1; // Reset volume
        }
    }

    private IEnumerator PlayWithFadeIn(AudioSource source, float fadeDuration)
    {
        if (isFading || source == null)
            yield break;

        isFading = true;

        // Dừng tất cả các track khác ngoại trừ track mới
        StopAllTracksExcept(source);

        // Cài đặt và phát track mới
        source.volume = 0;
        source.loop = (source == missionBGMs[currentMissionBGMIndex]) ? loopTracks[currentMissionBGMIndex] : false;
        source.Play();

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }

        source.volume = 1;

        isFading = false;
        isTrackPlaying = true;
    }

    private IEnumerator FadeOutBGM(AudioSource source, float fadeDuration)
    {
        if (isFading || source == null)
            yield break;

        isFading = true;
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
        isTrackPlaying = false;
        isFading = false;
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