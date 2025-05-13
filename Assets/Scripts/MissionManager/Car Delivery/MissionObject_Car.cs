using System;
using UnityEngine;

public class MissionObject_Car : MonoBehaviour
{
    public static event Action OnCarDelivery;

    public void InvokeCarDelivery() => OnCarDelivery?.Invoke();
}
