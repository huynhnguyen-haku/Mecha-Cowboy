using UnityEngine;

[CreateAssetMenu(fileName = "Car Delivery Mission", menuName = "Mission/Car Delivery - Mission")]

public class Mission_CarDelivery : Mission
{
    private bool isCarDelivered;
    public override void StartMission()
    {

        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

        isCarDelivered = false;
        MissionObject_Car.OnCarDelivery += CompleteCarDelivery;

        Car[] cars = Object.FindObjectsByType<Car>(FindObjectsSortMode.None); // Find all objects with "Car" script Component  

        foreach (Car car in cars)
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
    }

}
