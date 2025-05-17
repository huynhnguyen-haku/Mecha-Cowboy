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
        PlayMainMenuBGM(); // Phát track main menu khi bắt đầu
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

        // Nếu playBgm tắt, dừng BGM nếu đang phát
        if (!playBgm && isTrackPlaying && !isFading)
        {
            StopAllBGM();
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

    // Phát BGM cho main menu
    public void PlayMainMenuBGM(float fadeDuration = 1f)
    {
        if (mainMenuBGM == null)
        {
            Debug.LogWarning("AudioManager: Main menu BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.MainMenu;
        StartCoroutine(CrossfadeToSingleBGM(mainMenuBGM, fadeDuration));
    }

    // Phát BGM ngẫu nhiên cho mission
    public void PlayRandomMissionBGM(float fadeDuration = 1f)
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
        StartCoroutine(CrossfadeToSingleBGM(missionBGMs[currentMissionBGMIndex], fadeDuration));
    }

    // Phát BGM khi game over
    public void PlayGameOverBGM(float fadeDuration = 1f)
    {
        if (gameOverBGM == null)
        {
            Debug.LogWarning("AudioManager: Game over BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.GameOver;
        StartCoroutine(CrossfadeToSingleBGM(gameOverBGM, fadeDuration));
    }

    // Phát BGM khi mission complete
    public void PlayMissionCompleteBGM(float fadeDuration = 1f)
    {
        if (missionCompleteBGM == null)
        {
            Debug.LogWarning("AudioManager: Mission complete BGM is not assigned!");
            return;
        }

        currentBGMState = BGMState.MissionComplete;
        StartCoroutine(CrossfadeToSingleBGM(missionCompleteBGM, fadeDuration));
    }

    public void StopAllBGM(float fadeDuration = 1f)
    {
        AudioSource currentSource = GetCurrentPlayingBGM();
        if (currentSource != null)
        {
            StartCoroutine(FadeOutBGM(currentSource, fadeDuration));
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

    private IEnumerator CrossfadeToSingleBGM(AudioSource newSource, float fadeDuration)
    {
        if (isFading || newSource == null)
            yield break;

        isFading = true;

        // Fade out track hiện tại nếu đang phát
        AudioSource oldSource = GetCurrentPlayingBGM();

        // Dừng tất cả các track khác ngoại trừ track mới
        StopAllTracksExcept(newSource);

        // Cài đặt track mới
        newSource.volume = 0;
        newSource.loop = (newSource == missionBGMs[currentMissionBGMIndex]) ? loopTracks[currentMissionBGMIndex] : false;
        newSource.Play();

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            if (oldSource != null && oldSource != newSource)
            {
                oldSource.volume = Mathf.Lerp(oldSource.volume, 0, t);
            }
            newSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        if (oldSource != null && oldSource != newSource)
        {
            oldSource.Stop();
            oldSource.volume = 1; // Reset volume
        }
        newSource.volume = 1;

        isFading = false;
        isTrackPlaying = true;
    }

    private IEnumerator FadeOutBGM(AudioSource source, float fadeDuration = 1f)
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