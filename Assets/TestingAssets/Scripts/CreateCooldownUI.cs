using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateCooldownUI : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Lunge Cooldown UI", false, 12)]
    static void CreateLungeCooldownUI()
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

        // Create Cooldown Panel
        GameObject cooldownPanel = new GameObject("LungeCooldownPanel");
        cooldownPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = cooldownPanel.AddComponent<RectTransform>();
        // Position at bottom center
        panelRect.anchorMin = new Vector2(0.5f, 0.1f);
        panelRect.anchorMax = new Vector2(0.5f, 0.1f);
        panelRect.sizeDelta = new Vector2(150f, 150f);
        panelRect.anchoredPosition = Vector2.zero;

        // Add canvas group for fading
        cooldownPanel.AddComponent<CanvasGroup>();

        // Background circle
        GameObject background = new GameObject("Background");
        background.transform.SetParent(cooldownPanel.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        bgImage.sprite = GetCircleSprite();

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Cooldown Fill (radial)
        GameObject fill = new GameObject("CooldownFill");
        fill.transform.SetParent(cooldownPanel.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(1f, 0.3f, 0.3f, 0.8f);
        fillImage.sprite = GetCircleSprite();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Radial360;
        fillImage.fillOrigin = (int)Image.Origin360.Top;
        fillImage.fillClockwise = false;
        fillImage.fillAmount = 1f;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0.1f, 0.1f);
        fillRect.anchorMax = new Vector2(0.9f, 0.9f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Cooldown Text
        GameObject textObj = new GameObject("CooldownText");
        textObj.transform.SetParent(cooldownPanel.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "2.0s";
        text.fontSize = 36;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Add LungeCooldownUI component
        LungeCooldownUI cooldownUI = cooldownPanel.AddComponent<LungeCooldownUI>();
        cooldownUI.cooldownPanel = cooldownPanel;
        cooldownUI.cooldownFillImage = fillImage;
        cooldownUI.cooldownText = text;
        cooldownUI.playerController = FindObjectOfType<PlayerController>();

        Selection.activeGameObject = cooldownPanel;
        Debug.Log("Lunge Cooldown UI created successfully!");
    }

    static Sprite GetCircleSprite()
    {
        // Try to find a circle sprite in built-in resources
        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == "Knob" || sprite.name == "Circle" || sprite.name == "UISprite")
            {
                return sprite;
            }
        }

        // Fallback: return null and Unity will use default white sprite
        return null;
    }
#endif
}