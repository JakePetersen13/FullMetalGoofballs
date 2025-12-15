using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateBBQHUD : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create BBQ Health HUD", false, 13)]
    static void CreateBBQHealthHUD()
    {
        // Find canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create BBQ HUD Panel
        GameObject hudPanel = new GameObject("BBQHealthHUD");
        hudPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.sizeDelta = new Vector2(0, 100);
        panelRect.anchoredPosition = new Vector2(0, -50);

        // === PLAYER BBQ (Left Side) ===
        CreateBBQHealthBar(hudPanel, "PlayerBBQHealth", new Vector2(0.02f, 0.5f), new Vector2(0.4f, 0.5f),
            new Color(0.2f, 0.6f, 1f), "PLAYER BBQ", out Slider playerSlider, out TextMeshProUGUI playerText, out Image playerFill);

        // === ENEMY BBQ (Right Side) ===
        CreateBBQHealthBar(hudPanel, "EnemyBBQHealth", new Vector2(0.6f, 0.5f), new Vector2(0.98f, 0.5f),
            new Color(1f, 0.3f, 0.2f), "ENEMY BBQ", out Slider enemySlider, out TextMeshProUGUI enemyText, out Image enemyFill);

        // Add BBQHealthHUD component
        BBQHealthHUD healthHUD = hudPanel.AddComponent<BBQHealthHUD>();
        healthHUD.playerHealthSlider = playerSlider;
        healthHUD.playerHealthText = playerText;
        healthHUD.playerFillImage = playerFill;
        healthHUD.enemyHealthSlider = enemySlider;
        healthHUD.enemyHealthText = enemyText;
        healthHUD.enemyFillImage = enemyFill;

        // Auto-find barbecues
        Barbecue[] barbecues = FindObjectsOfType<Barbecue>();
        foreach (Barbecue bbq in barbecues)
        {
            if (bbq.team == Team.Player)
            {
                healthHUD.playerBarbecue = bbq;
            }
            else if (bbq.team == Team.Enemy)
            {
                healthHUD.enemyBarbecue = bbq;
            }
        }

        Selection.activeGameObject = hudPanel;
        Debug.Log("BBQ Health HUD created successfully!");
    }

    static void CreateBBQHealthBar(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax,
        Color fillColor, string label, out Slider slider, out TextMeshProUGUI healthText, out Image fillImage)
    {
        // Container
        GameObject container = new GameObject(name);
        container.transform.SetParent(parent.transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = anchorMin;
        containerRect.anchorMax = anchorMax;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(container.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 18;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.fontStyle = FontStyles.Bold;

        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.6f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        // Slider
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(container.transform, false);

        slider = sliderObj.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.05f, 0.2f);
        sliderRect.anchorMax = new Vector2(0.7f, 0.5f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        // Slider Background
        GameObject sliderBg = new GameObject("SliderBackground");
        sliderBg.transform.SetParent(sliderObj.transform, false);
        Image sliderBgImage = sliderBg.AddComponent<Image>();
        sliderBgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        RectTransform sliderBgRect = sliderBg.GetComponent<RectTransform>();
        sliderBgRect.anchorMin = Vector2.zero;
        sliderBgRect.anchorMax = Vector2.one;
        sliderBgRect.offsetMin = Vector2.zero;
        sliderBgRect.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Health Text
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(container.transform, false);

        healthText = textObj.AddComponent<TextMeshProUGUI>();
        healthText.text = "200 / 200";
        healthText.fontSize = 22;
        healthText.color = Color.white;
        healthText.alignment = TextAlignmentOptions.Center;
        healthText.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.7f, 0.1f);
        textRect.anchorMax = new Vector2(1f, 0.6f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
#endif
}