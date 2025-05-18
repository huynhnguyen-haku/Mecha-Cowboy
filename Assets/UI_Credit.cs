using UnityEngine;

public class UI_Credit : MonoBehaviour
{
    [SerializeField] private RectTransform creditText;

    private float scrollSpeed = 100f;

    private bool isScrolling = true;

    private void Update()
    {
        if (isScrolling)
        {
            creditText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }
}
