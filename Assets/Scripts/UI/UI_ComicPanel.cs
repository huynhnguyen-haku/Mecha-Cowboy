using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image[] comicPanel;
    [SerializeField] private int imageIndex;
    [SerializeField] private GameObject playButton;

    private Image myImage;
    [SerializeField] private bool isComicFinished;

    private void Start()
    {
        myImage = GetComponent<Image>();
        ShowNextImage();
    }

    private void ShowNextImage()
    {
        if (isComicFinished) 
            return;

        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float timeElapsed = 0f;
        Color currentColor = comicPanel[imageIndex].color;
        float startAlpha = currentColor.a;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);

            comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        imageIndex++;

        if (imageIndex >= comicPanel.Length)
        {
            EnablePlayButton();
        }

        onComplete?.Invoke();
    }
    private void EnablePlayButton()
    {
        StopAllCoroutines();
        isComicFinished = true;
        playButton.SetActive(true);
        myImage.raycastTarget = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        // Check if the current image index is out of bounds.
        if (imageIndex >= comicPanel.Length)
        {
            EnablePlayButton();
            return;
        }

        comicPanel[imageIndex].color = Color.white;
        imageIndex++;

        // Check again if the index is out of bounds after incrementing.
        if (imageIndex >= comicPanel.Length)
        {
            EnablePlayButton();
            return;
        }

        if (isComicFinished)
            return;

        ShowNextImage();
    }

}
