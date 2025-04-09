using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [Header("Camera Settings")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float targetCameraDistance;
    private float distanceChangeRate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance)
            return;

        float currentDistance = transposer.m_CameraDistance;
        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.1f)
            return;

        transposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => transposer.m_CameraDistance = distance;


}
