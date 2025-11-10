using UnityEngine;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
    private static FadeOverlay instance;
    public static FadeOverlay Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateFadeOverlay();
            }
            return instance;
        }
    }

    private Image fadeImage;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    static FadeOverlay CreateFadeOverlay()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // Render on top of everything

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create fade image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        Image image = imageObj.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0); // Start transparent

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        // Add FadeOverlay component
        FadeOverlay overlay = canvasObj.AddComponent<FadeOverlay>();
        overlay.fadeImage = image;

        DontDestroyOnLoad(canvasObj);

        return overlay;
    }

    public void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = Mathf.Clamp01(alpha);
            fadeImage.color = color;
        }
    }

    public void SetColor(Color color)
    {
        if (fadeImage != null)
        {
            fadeImage.color = color;
        }
    }
}