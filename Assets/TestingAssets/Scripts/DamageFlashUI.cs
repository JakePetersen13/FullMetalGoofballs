using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashUI : MonoBehaviour
{
    public Image flashImage;

    [Range(0f, 1f)]
    public float defaultAlpha = 0.5f;

    public float fadeOutDuration = 0.2f;

    Coroutine flashRoutine;

    void Awake()
    {
        SetTransparent();
    }

    //Instant flash that fades out
    public void Flash()
    {
        KeepOn(defaultAlpha);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FadeOut());
    }

    // Keep red overlay ON (no fade)
    public void KeepOn(float alpha = -1f)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (alpha < 0f)
            alpha = defaultAlpha;

        flashImage.color = new Color(1f, 0f, 0f, alpha);
    }

    //Instantly turn OFF (fully transparent)
    public void SetTransparent()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        flashImage.color = new Color(1f, 0f, 0f, 0f);
    }

    IEnumerator FadeOut()
    {
        float startAlpha = flashImage.color.a;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0f, t / fadeOutDuration);
            flashImage.color = new Color(1f, 0f, 0f, a);
            yield return null;
        }

        SetTransparent();
    }
}
