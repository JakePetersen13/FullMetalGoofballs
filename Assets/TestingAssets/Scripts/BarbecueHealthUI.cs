using UnityEngine;
using UnityEngine.UI;

public class BarbecueHealthUI : MonoBehaviour
{
    [HideInInspector] public Barbecue barbecue;
    [HideInInspector] public Slider healthSlider;
    [HideInInspector] public Image fillImage;
    [HideInInspector] public Canvas canvas;

    private RectTransform rectTransform;

    [Header("---Colors---")]
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("---Size---")]
    public Vector2 healthBarSize = new Vector2(300f, 45f);

    public void Initialize(Barbecue targetBarbecue, Vector3 offset)
    {
        barbecue = targetBarbecue;

        // Find or create BBQCanvas
        Canvas bbqCanvas = FindBBQCanvas();

        // Set this GameObject's parent to BBQCanvas
        transform.SetParent(bbqCanvas.transform, false);

        // Setup canvas for world space
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;

        // Setup rect transform
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = healthBarSize;

        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Create slider
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(transform, false);
        healthSlider = sliderObj.AddComponent<Slider>();

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.05f, 0.2f);
        sliderRect.anchorMax = new Vector2(0.95f, 0.8f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        // Create slider background
        GameObject sliderBg = new GameObject("SliderBackground");
        sliderBg.transform.SetParent(sliderObj.transform, false);
        Image sliderBgImage = sliderBg.AddComponent<Image>();
        sliderBgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        RectTransform sliderBgRect = sliderBg.GetComponent<RectTransform>();
        sliderBgRect.anchorMin = Vector2.zero;
        sliderBgRect.anchorMax = Vector2.one;
        sliderBgRect.offsetMin = Vector2.zero;
        sliderBgRect.offsetMax = Vector2.zero;

        // Create fill area
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        fillImage = fill.AddComponent<Image>();
        fillImage.color = fullHealthColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        healthSlider.fillRect = fillRect;
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
        healthSlider.value = 1f;

        // Position above barbecue
        transform.position = barbecue.transform.position + offset;

        // Scale down for world space
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    Canvas FindBBQCanvas()
    {
        // First, try to find existing BBQCanvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in allCanvases)
        {
            if (c.gameObject.name == "BBQCanvas")
            {
                return c;
            }
        }

        // If not found, create BBQCanvas
        GameObject bbqCanvasObj = new GameObject("BBQCanvas");
        Canvas bbqCanvas = bbqCanvasObj.AddComponent<Canvas>();
        bbqCanvas.renderMode = RenderMode.WorldSpace;
        bbqCanvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        Debug.Log("Created BBQCanvas for barbecue health bars");
        return bbqCanvas;
    }

    void LateUpdate()
    {
        if (barbecue != null)
        {
            // Follow barbecue position
            transform.position = barbecue.transform.position + barbecue.healthBarOffset;

            // Face camera
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                 Camera.main.transform.rotation * Vector3.up);
            }
        }
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if (healthSlider == null || fillImage == null) return;

        float healthPercent = Mathf.Clamp01(currentHP / maxHP);

        // Update slider
        healthSlider.value = healthPercent;
        fillImage.fillAmount = healthPercent;

        // Update color based on health percentage
        if (healthPercent <= 0.25f)
        {
            fillImage.color = lowHealthColor;
        }
        else if (healthPercent <= 0.5f)
        {
            fillImage.color = Color.Lerp(lowHealthColor, midHealthColor, (healthPercent - 0.25f) / 0.25f);
        }
        else
        {
            fillImage.color = Color.Lerp(midHealthColor, fullHealthColor, (healthPercent - 0.5f) / 0.5f);
        }
    }
}