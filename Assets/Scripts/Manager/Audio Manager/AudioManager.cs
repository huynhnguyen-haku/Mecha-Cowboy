using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;

    private int bgmIndex;

    private void Start()
    {
        // Ensure all AudioSources have playOnAwake disabled at the start of the game
        foreach (var audioSource in bgm)
        {
            audioSource.playOnAwake = false;
            audioSource.Stop(); // Stop any audio that might have started playing
        }
    }

    private void Update()
    {
        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();

        else if (bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();
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
}
