using System.Collections;
using UnityEngine;

public class Car_SFX : MonoBehaviour
{
    private Car_Controller carController;

    [SerializeField] private AudioSource engineStart;
    [SerializeField] private AudioSource engineIdle;
    [SerializeField] private AudioSource engineStop;

    private float minSpeed = 0;
    private float maxSpeed = 20;

    public float minPitch = 0.75f;
    public float maxPitch = 1.5f;

    private bool enableCarSFX;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        Invoke(nameof(EnAbleCarSFX), 1f);
    }

    private void Update()
    {
        UpdateEngineSFX();
    }

    private void UpdateEngineSFX()
    {
        // Lấy giá trị tuyệt đối của tốc độ để đảm bảo luôn dương
        float currentSpeed = Mathf.Abs(carController.speed) / 3.6f;

        // Tính toán pitch dựa trên tốc độ hiện tại
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
            StartCoroutine(PlayEngineStartThenIdle());
        }
        else
        {
            engineIdle.Stop();
            engineStop.Play();
        }
    }

    private IEnumerator PlayEngineStartThenIdle()
    {
        // Play engine start sound
        engineStart.Play();

        // Wait for the engine start sound to finish
        yield return new WaitForSeconds(engineStart.clip.length);

        // Play engine idle sound
        engineIdle.Play();
    }

    private void EnAbleCarSFX()
    {
        enableCarSFX = true;
    }
}
