using UnityEngine;

[CreateAssetMenu(fileName = "Car Delivery Mission", menuName = "Mission/Car Delivery - Mission")]

public class Mission_CarDelivery : Mission
{
    public bool isCarDelivered;

    private void OnEnable()
    {
        isCarDelivered = false;
    }

    public override void StartMission()
    {
        if (isCarDelivered)
            return;

        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);
        MissionObject_CarDeliveryZone deliveryZone = FindObjectOfType<MissionObject_CarDeliveryZone>();
        if (deliveryZone != null)
        {
            PathfindingIndicator pathfindingIndicator = FindObjectOfType<PathfindingIndicator>();
            if (pathfindingIndicator != null)
            {
                pathfindingIndicator.SetTarget(deliveryZone.transform);
                Debug.Log("Mission_CarDelivery: Set PathfindingIndicator target to MissionObject_CarDeliveryZone.");
            }
            else
            {
                Debug.LogWarning("Mission_CarDelivery: PathfindingIndicator not found in scene!");
            }


            string missionText = "Find a functiuonal car";
            string missionDetails = "Get to the car and drive it to the specified parking area";

            UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);

            MissionObject_Car.OnCarDelivery += CompleteCarDelivery;
            Car_Controller[] cars = Object.FindObjectsByType<Car_Controller>(FindObjectsSortMode.None);

            foreach (var car in cars)
            {
                if (car != null)
                {
                    car.gameObject.AddComponent<MissionObject_Car>();
                }
            }
        }
    }

    public override bool MissionCompleted()
    {
        return isCarDelivered;
    }

    private void CompleteCarDelivery()
    {
        isCarDelivered = true;
        MissionObject_Car.OnCarDelivery -= CompleteCarDelivery;

        string missionText = "Car delivered.";
        string missionDetails = "Get to the evacuation point to complete mission";
        UI.instance.inGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override MissionType GetMissionType()
    {
        return MissionType.CarDelivery;
    }
}
