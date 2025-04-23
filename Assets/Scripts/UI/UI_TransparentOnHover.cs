using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TransparentOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>();
    private Dictionary<TextMeshProUGUI, Color> originalTextColors = new Dictionary<TextMeshProUGUI, Color>();

    private bool hasWeaponSlots;
    private Player_WeaponController playerWeaponController;

    private void Start()
    {
        hasWeaponSlots = GetComponentInChildren<UI_WeaponSlot>();
        if (hasWeaponSlots)
        {
            playerWeaponController = FindObjectOfType<Player_WeaponController>();
        }

        // Check Image components and their original colors
        foreach (var image in GetComponentsInChildren<Image>(true))
        {
            originalImageColors[image] = image.color;
        }

        // Check TextMeshProUGUI components and their original colors
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            originalTextColors[text] = text.color;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Set transparent color for images
        foreach (var image in originalImageColors.Keys)
        {
            var color = image.color;
            color.a = 0.15f;
            image.color = color;
        }

        // Set transparent color for text
        foreach (var text in originalTextColors.Keys)
        {
            var color = text.color;
            color.a = 0.15f;
            text.color = color;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Restore original colors for images
        foreach (var image in originalImageColors.Keys)
        {
            image.color = originalImageColors[image];
        }

        // Restore original colors for text
        foreach (var text in originalTextColors.Keys)
        {
            text.color = originalTextColors[text];
        }

        playerWeaponController?.UpdateWeaponUI();
    }
}
