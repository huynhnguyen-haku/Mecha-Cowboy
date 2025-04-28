using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_Controller carController;
    private Transform player;

    private float defaultPlayerScale;

    [Header("Exit Points Settings")]
    [SerializeField] private Transform[] exitPoints;

    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        player = GameManager.instance.player.transform;
    }

    public override void Interact()
    {
        base.Interact();
        EnterCar();
    }

    private void EnterCar()
    {
        ControlsManager.instance.SwitchToCarControls();
        carController.ActivateCar(true);

        defaultPlayerScale = player.localScale.x;

        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform);
    }

    public void ExitCar()
    {
        if (!carController.carActive)
            return;

        carController.ActivateCar(false);

        player.parent = null;
        player.position = GetExitPoint();
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);
        ControlsManager.instance.SwitchToCharacterControls();
        Player_AimController aim = GameManager.instance.player.aim;

        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget());
    }


    private Vector3 GetExitPoint()
    {
        foreach (var exitPoint in exitPoints)
        {
            var trigger = exitPoint.GetComponent<ExitPointTrigger>();
            if (trigger != null && !trigger.isBlocked)
            {
                return exitPoint.position; // Trả về điểm thoát không bị chặn
            }
        }

        // Nếu tất cả các điểm đều bị chặn, hiển thị cảnh báo và trả về vị trí fallback
        Debug.LogWarning("No valid exit point found. Returning fallback position.");
        return transform.position + transform.right * 2; // Vị trí fallback (bên phải xe)
    }
}
