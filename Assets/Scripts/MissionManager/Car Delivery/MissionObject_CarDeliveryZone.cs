using UnityEngine;

public class MissionObject_CarDeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Car_Controller>() != null)
        {
            Car_Controller car = other.GetComponent<Car_Controller>();

            if (car != null)
                car.GetComponent<MissionObject_Car>().InvokeCarDelivery();
            
        }
    }
}

