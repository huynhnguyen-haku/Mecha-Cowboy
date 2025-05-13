using UnityEngine;
using UnityEngine.AI;

public class Car_Interaction : Interactable
{
    private Car_HealthController carHealthController;
    private Car_Controller carController;
    private Transform player;
    private PathfindingIndicator pathIndicator; // Tham chiếu đến PathfindingIndicator
    private NavMeshObstacle carObstacle; // Tham chiếu đến NavMeshObstacle của xe

    private float defaultPlayerScale;

    [Header("Exit Points Settings")]
    [SerializeField] private Transform[] exitPoints;

    private void Start()
    {
        carHealthController = GetComponent<Car_HealthController>();
        carController = GetComponent<Car_Controller>();
        player = GameManager.instance.player.transform;
        pathIndicator = player.GetComponent<PathfindingIndicator>(); // Tìm PathfindingIndicator trên người chơi
        carObstacle = GetComponent<NavMeshObstacle>(); // Lấy NavMeshObstacle của xe
    }

    public override void Interact()
    {
        if (carHealthController.carBroken)
            return;

        base.Interact();
        EnterCar();
    }

    public override void Highlight(bool active)
    {
        if (carHealthController.carBroken)
            return;

        base.Highlight(active);
    }

    private void EnterCar()
    {
        ControlsManager.instance.SwitchToCarControls();
        carController.ActivateCar(true);

        // Gán chiếc xe hiện tại vào GameManager
        GameManager.instance.currentCar = carController;
        carHealthController.UpdateCarHealthUI();

        // Set bool cho player movement
        GameManager.instance.player.GetComponent<Player_Movement>().isInCar = true;

        defaultPlayerScale = player.localScale.x;
        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Make the player a child of the car
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform, 16, 0.5f);

        // Tắt NavMeshObstacle khi lên xe
        if (carObstacle != null)
        {
            carObstacle.enabled = false;
        }
    }

    public void ExitCar()
    {
        if (!carController.carActive)
            return;

        carController.ActivateCar(false);
        carController.DecelerateCar();

        // Set the car controller to null
        GameManager.instance.currentCar = null;

        // Set bool for player movement
        GameManager.instance.player.GetComponent<Player_Movement>().isInCar = false;

        player.parent = null;
        player.position = GetExitPoint();
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);

        ControlsManager.instance.SwitchToCharacterControls();
        Player_AimController aim = GameManager.instance.player.aim;

        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f);

        // Bật lại NavMeshObstacle khi xuống xe
        if (carObstacle != null)
        {
            carObstacle.enabled = true;
        }
    }

    // Check if the exit doors is blocked.
    private Vector3 GetExitPoint()
    {
        foreach (var exitPoint in exitPoints)
        {
            var trigger = exitPoint.GetComponent<Door_ExitPoint>();

            if (trigger != null && !trigger.isBlocked)
                return exitPoint.position;

        }
        // If both exit doors are blocked, return the default one.
        return transform.position + transform.up * 2;
    }
}