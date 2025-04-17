using System.Collections;
using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private BoxCollider zoneCollider;
    private MeshRenderer meshRenderer;
    private Coroutine fadeCoroutine;
    private bool isPlayerInside = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        zoneCollider = GetComponent<BoxCollider>();
        ActivateWall(false);
    }

    private void ActivateWall(bool activate)
    {
        zoneCollider.isTrigger = !activate;
    }

    private IEnumerator WallActivation()
    {
        ActivateWall(true);
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMeshRenderer(true));

        yield return new WaitForSeconds(1f);

        if (!isPlayerInside)
        {
            ActivateWall(false);
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeMeshRenderer(false));
        }
    }

    private IEnumerator FadeMeshRenderer(bool fadeIn)
    {
        float duration = 1f;
        float elapsed = 0f;
        meshRenderer.enabled = true;
        Color color = meshRenderer.material.color;
        float startAlpha = fadeIn ? 0f : color.a;
        float endAlpha = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            meshRenderer.material.color = color;
            yield return null;
        }

        color.a = endAlpha;
        meshRenderer.material.color = color;
        if (!fadeIn)
            meshRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInside = true;
        StartCoroutine(WallActivation());
        Debug.Log("We don't have any business in that area! Don't go near it!");
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMeshRenderer(false));
        ActivateWall(false);
    }
}
