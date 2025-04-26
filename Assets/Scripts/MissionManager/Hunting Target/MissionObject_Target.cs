using System;
using UnityEngine;

public class MissionObject_Target : MonoBehaviour
{
    public static event Action OnTargetKilled;

    public void InvokeOnTargetKilled()
    {
        Debug.Log($"Target killed: {gameObject.name}");
        OnTargetKilled?.Invoke();
    }
}
