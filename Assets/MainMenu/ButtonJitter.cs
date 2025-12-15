using UnityEngine;
using System.Collections;

public class ButtonJitter : MonoBehaviour
{
    RectTransform rect;
    Vector2 originalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
    }

    public void StartJitter()
    {
        StopAllCoroutines();
        StartCoroutine(Jitter());
    }

    public void StopJitter()
    {
        StopAllCoroutines();
        rect.anchoredPosition = originalPos;
    }

    IEnumerator Jitter()
    {
        while (true)
        {
            rect.anchoredPosition = originalPos + Random.insideUnitCircle * 2.5f;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
