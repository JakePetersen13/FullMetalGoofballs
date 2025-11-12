using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateHealthUI : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Health Bar", false, 10)]
    static void CreateHealthBarUI()
    {
        // Create Canvas if none exists
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create Health Bar Panel
        GameObject healthBarPanel = new GameObject("HealthBarPanel");
        healthBarPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = healthBarPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.02f, 0.92f);
        panelRect.anchorMax = new Vector2(0.35f, 0.98f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarPanel.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Health Slider
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(healthBarPanel.transform, false);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.05f, 0.3f);
        sliderRect.anchorMax = new Vector2(0.7f, 0.7f);
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
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;

        // Health Text
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(healthBarPanel.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "100 / 100";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.7f, 0);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Add PlayerHealthUI component
        PlayerHealthUI healthUI = healthBarPanel.AddComponent<PlayerHealthUI>();
        healthUI.healthSlider = slider;
        healthUI.healthText = text;
        healthUI.fillImage = fillImage;
        healthUI.playerController = FindObjectOfType<PlayerController>();

        Selection.activeGameObject = healthBarPanel;
        Debug.Log("Health Bar UI created successfully!");
    }
#endif
}