using System.Collections;
using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private BoxCollider zoneCollider;
    private MeshRenderer meshRenderer;
    private Coroutine fadeCoroutine;
    private bool isPlayerInside;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        zoneCollider = GetComponent<BoxCollider>();
        SetWallState(false, false);
    }

    private void SetWallState(bool activate, bool fadeIn)
    {
        zoneCollider.isTrigger = !activate;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMeshRenderer(fadeIn));
    }

    private IEnumerator FadeMeshRenderer(bool fadeIn)
    {
        float duration = 1f, elapsed = 0f;
        meshRenderer.enabled = true;
        Color color = meshRenderer.material.color;
        float startAlpha = fadeIn ? 0f : color.a, endAlpha = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            meshRenderer.material.color = color;
            yield return null;
        }

        color.a = endAlpha;
        meshRenderer.material.color = color;
        meshRenderer.enabled = fadeIn;
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInside = true;
        SetWallState(true, true);
        Debug.Log("We don't have any business in that area! Don't go near it!");
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
        SetWallState(false, false);
    }
}
