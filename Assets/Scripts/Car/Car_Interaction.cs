using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_HealthController carHealthController;
    private Car_Controller carController;
    private Transform player;

    private float defaultPlayerScale;

    [Header("Exit Points Settings")]
    [SerializeField] private Transform[] exitPoints;

    private void Start()
    {
        carHealthController = GetComponent<Car_HealthController>();
        carController = GetComponent<Car_Controller>();
        player = GameManager.instance.player.transform;
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
        carHealthController.UpdateCarHealthUI();
        carController.ActivateCar(true);

        // Set bool for player movement
        GameManager.instance.player.GetComponent<Player_Movement>().isInCar = true;

        defaultPlayerScale = player.localScale.x;
        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Make the player a child of the car
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform, 12, 0.5f);
    }

    public void ExitCar()
    {
        if (!carController.carActive)
            return;

        carController.ActivateCar(false);
        carController.DecelerateCar();

        // Set bool for player movement
        GameManager.instance.player.GetComponent<Player_Movement>().isInCar = false;

        player.parent = null;
        player.position = GetExitPoint();
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);

        ControlsManager.instance.SwitchToCharacterControls();
        Player_AimController aim = GameManager.instance.player.aim;

        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f);
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
