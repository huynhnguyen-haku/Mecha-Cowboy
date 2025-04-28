using UnityEngine;

public class ExitPointTrigger : MonoBehaviour
{
    public bool isBlocked { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        // ?ánh d?u là b? ch?n n?u có v?t th? ?i vào vùng trigger
        isBlocked = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // ?ánh d?u là không b? ch?n n?u v?t th? r?i kh?i vùng trigger
        isBlocked = false;
    }
}
