using UnityEngine;

[CreateAssetMenu(fileName = "Car Delivery Mission", menuName = "Mission/Car Delivery - Mission")]

public class Mission_CarDelivery : Mission
{
    public bool isCarDelivered;
    public override void StartMission()
    {
        reward = 100;
        isCarDelivered = false;

        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

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
