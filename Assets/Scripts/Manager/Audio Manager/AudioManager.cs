using System.Collections;
using Unity.Hierarchy;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;
    [SerializeField] private int bgmIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayBGM(0);
    }

    private void Update()
    {
        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();

        if(playBgm && bgm[bgmIndex].isPlaying == false)
        {
            PlayRandomBGM();
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

    public void ControlSFX_FadeAndDelay(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1 )
    {
        if (source == null)
            return;
        StartCoroutine(ProcessSFX_FadeAndDelay(source, play, targetVolume, delay, fadeDuration));
    }

    public void PlayBGM(int index)
    {
        StopAllBGM();

        bgmIndex = index;
        bgm[index].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    [ContextMenu("Play Random BGM")]
    public void PlayRandomBGM()
    {
        StopAllBGM();
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }

        return false;
    }

    private IEnumerator ProcessSFX_FadeAndDelay(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1 )
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

        // Fade in or fade out over the duration
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
