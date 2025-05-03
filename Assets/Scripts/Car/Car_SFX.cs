using UnityEngine;

public class Car_SFX : MonoBehaviour
{
    private Car_Controller carController;

    [SerializeField] private AudioSource engineStart;
    [SerializeField] private AudioSource engineIdle;
    [SerializeField] private AudioSource engineStop;

    [SerializeField] private AudioSource tireSqueal;

    private float minSpeed = 0;
    private float maxSpeed = 20;

    public float minPitch = 0.75f;
    public float maxPitch = 1.5f;

    private bool enableCarSFX;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        Invoke(nameof(EnableCarSFX), 1f);
    }

    private void Update()
    {
        UpdateEngineSFX();
    }

    private void UpdateEngineSFX()
    {
        float currentSpeed = Mathf.Abs(carController.speed) / 3.6f;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / maxSpeed);

        // Áp dụng pitch cho âm thanh động cơ
        engineIdle.pitch = pitch;
    }


    public void ActivateCarSFX(bool active)
    {
        if (enableCarSFX == false)
            return;

        if (active)
        {
            engineStart.Play();
            AudioManager.instance.ControlSFX_FadeAndDelay(engineIdle, true, 0.3f, 1);
        }
        else
        {
            AudioManager.instance.ControlSFX_FadeAndDelay(engineIdle, false, 0f, 0.25f);
            engineStop.Play();
        }
    }

    public void HandleTireSqueal(bool isDrifting)
    {
        if (!enableCarSFX || tireSqueal == null)
            return;

        if (isDrifting)
        {
            // Chỉ phát âm thanh nếu nó chưa đang phát
            if (!tireSqueal.isPlaying)
            {
                tireSqueal.volume = 0.3f; // Đảm bảo âm lượng được khôi phục
                tireSqueal.Play();
            }
        }
        else
        {
            // Fade out âm thanh tire squeal khi ngừng drift
            if (tireSqueal.isPlaying)
            {
                AudioManager.instance.ControlSFX_FadeAndDelay(tireSqueal, false, 0f, 0.25f, 0.25f);
            }
        }
    }

    private void EnableCarSFX()
    {
        enableCarSFX = true;
    }
}
