using UnityEngine;

public class MinimapSprite : MonoBehaviour
{
    private void Start()
    {
        // Đặt góc quay ban đầu
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void LateUpdate()
    {
        // Luôn giữ góc quay cố định (90, 0, 0) bất kể parent xoay
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}