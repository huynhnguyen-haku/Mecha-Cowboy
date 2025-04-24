using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Mouse Hover Settings")]
    public float scaleSpeed = 1;
    public float scaleRate;

    private Vector3 defaultScale;
    private Vector3 targetScale;

    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    public virtual void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;

        buttonImage = GetComponentInChildren<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void Update()
    {
        if (Mathf.Abs(transform.lossyScale.x - targetScale.x) > 0.01f)
        {
            float scaleValue = Mathf.Lerp(transform.localScale.x, targetScale.x, Time.deltaTime * scaleSpeed);

            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = defaultScale * scaleRate;

        if (buttonImage != null)
            buttonImage.color = Color.yellow;

        if (buttonText != null)
            buttonText.color = Color.yellow;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ResetToDefaultAppearance();
    }


    public virtual void OnPointerDown(PointerEventData eventData)
    {
        ResetToDefaultAppearance();
    }

    private void ResetToDefaultAppearance()
    {
        targetScale = defaultScale;

        if (buttonImage != null)
            buttonImage.color = Color.white;

        if (buttonText != null)
            buttonText.color = Color.white;
    }
}
