using UnityEngine;

public class UI_Credit : MonoBehaviour
{
    [SerializeField] private RectTransform creditText;

    private float scrollSpeed = 150f;
    private bool isScrolling = true;
    private Vector2 startPosition; // Vị trí ban đầu của creditText

    private void Awake()
    {
        // Gán vị trí ban đầu khi UI được khởi tạo
        startPosition = creditText.anchoredPosition;
    }

    private void OnEnable()
    {
        // Reset vị trí về ban đầu khi UI được kích hoạt
        creditText.anchoredPosition = startPosition;
        isScrolling = true; // Bắt đầu cuộn lại
    }

    private void OnDisable()
    {
        // Tạm dừng cuộn khi UI bị disable
        isScrolling = false;
    }

    private void Update()
    {
        if (isScrolling)
        {
            creditText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }
}