using UnityEngine;

public class ExitPointTrigger : MonoBehaviour
{
    public bool isBlocked { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        // ?�nh d?u l� b? ch?n n?u c� v?t th? ?i v�o v�ng trigger
        isBlocked = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // ?�nh d?u l� kh�ng b? ch?n n?u v?t th? r?i kh?i v�ng trigger
        isBlocked = false;
    }
}
