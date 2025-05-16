using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [SerializeField] private float resumeRate = 3;
    [SerializeField] private float pauseRate = 7;

    private float timeAdjustRate;
    private float targetTimeScale;

    private void Awake()
    {
        instance = this;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SlowMotionFor(1f);
        }

        if (Mathf.Abs(Time.timeScale - targetTimeScale) > 0.05f)
        {
            float adjustRate = Time.unscaledDeltaTime * timeAdjustRate;
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate);
        }
        else
        {
            Time.timeScale = targetTimeScale;
        }
    }

    public void PauseTime()
    {
        timeAdjustRate = pauseRate;
        targetTimeScale = 0;

        // Dừng nhân vật
        GameManager.instance.player.movement.SetPaused(true);
    }

    public void ResumeTime()
    {
        timeAdjustRate = resumeRate;
        targetTimeScale = 1;

        // Tiếp tục nhân vật
        GameManager.instance.player.movement.SetPaused(false);
    }


    public void SlowMotionFor(float duration)
    {
        StartCoroutine(SlowTime(duration));
    }

    private IEnumerator SlowTime(float duration)
    {
        targetTimeScale = 0.5f;
        Time.timeScale = targetTimeScale;
        yield return new WaitForSecondsRealtime(duration);
        ResumeTime();
    }
}
