using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateBBQHealthUI : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create BBQ Health Bar", false, 11)]
    static void CreateBBQHealthBarUI()
    {
        // Find or create BBQCanvas
        Canvas bbqCanvas = FindBBQCanvas();

        // Check if a Barbecue is selected
        Barbecue selectedBarbecue = Selection.activeGameObject?.GetComponent<Barbecue>();
        if (selectedBarbecue == null)
        {
            Debug.LogWarning("Please select a GameObject with a Barbecue component first!");
            return;
        }

        // Create Health Bar UI GameObject
        GameObject healthBarUI = new GameObject($"{selectedBarbecue.teamName}_HealthUI");
        healthBarUI.transform.SetParent(bbqCanvas.transform, false);

        // Add Canvas component for world space
        Canvas worldCanvas = healthBarUI.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;

        // Add RectTransform
        RectTransform mainRect = healthBarUI.GetComponent<RectTransform>();
        mainRect.sizeDelta = new Vector2(100f, 15f);
        mainRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Position above barbecue
        healthBarUI.transform.position = selectedBarbecue.transform.position + selectedBarbecue.healthBarOffset;

        // Create Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarUI.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Create Health Slider
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(healthBarUI.transform, false);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.05f, 0.2f);
        sliderRect.anchorMax = new Vector2(0.95f, 0.8f);
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
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Add BarbecueHealthUI component
        BarbecueHealthUI healthUI = healthBarUI.AddComponent<BarbecueHealthUI>();

        // Set up references directly
        healthUI.barbecue = selectedBarbecue;
        healthUI.healthSlider = slider;
        healthUI.fillImage = fillImage;
        healthUI.canvas = worldCanvas;

        // Link to Barbecue component
        selectedBarbecue.healthUI = healthUI;

        // Initialize the health display
        healthUI.UpdateHealth(selectedBarbecue.currentHP, selectedBarbecue.maxHP);

        Selection.activeGameObject = healthBarUI;
        Debug.Log($"BBQ Health Bar created for {selectedBarbecue.teamName}!");
    }

    static Canvas FindBBQCanvas()
    {
        // Try to find existing BBQCanvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in allCanvases)
        {
            if (c.gameObject.name == "BBQCanvas")
            {
                return c;
            }
        }

        // Create BBQCanvas if not found
        GameObject bbqCanvasObj = new GameObject("BBQCanvas");
        Canvas bbqCanvas = bbqCanvasObj.AddComponent<Canvas>();
        bbqCanvas.renderMode = RenderMode.WorldSpace;
        bbqCanvasObj.AddComponent<CanvasScaler>();

        Debug.Log("Created BBQCanvas for barbecue health bars");
        return bbqCanvas;
    }
#endif
}