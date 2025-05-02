using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;
    [SerializeField] private int bgmIndex;

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
}
