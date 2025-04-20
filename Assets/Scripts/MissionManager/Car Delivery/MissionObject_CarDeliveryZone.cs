using UnityEngine;

public class MissionObject_CarDeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Car>() != null)
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
            {
                car.GetComponent<MissionObject_Car>().InvokeCarDelivery();
            }
        }
    }
}

