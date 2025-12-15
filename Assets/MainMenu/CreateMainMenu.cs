using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateMainMenu : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Main Menu", false, 14)]
    static void CreateMainMenuUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.15f, 1f);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Create Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "FULL METAL\nGOOFBALLS";
        titleText.fontSize = 80;
        titleText.color = new Color(1f, 0.8f, 0.2f); // Gold color
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(800, 250);
        titleRect.anchoredPosition = Vector2.zero;

        // Create Description Text (below title)
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "SELECT A MODE";
        descText.fontSize = 28;
        descText.color = new Color(0.8f, 0.8f, 0.9f); // Light gray
        descText.alignment = TextAlignmentOptions.Center;
        descText.fontStyle = FontStyles.Italic;

        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.55f);
        descRect.anchorMax = new Vector2(0.5f, 0.55f);
        descRect.sizeDelta = new Vector2(900, 100);
        descRect.anchoredPosition = Vector2.zero;

        // Create Menu Panel
        GameObject menuPanel = new GameObject("MenuPanel");
        menuPanel.transform.SetParent(canvas.transform, false);

        RectTransform menuRect = menuPanel.AddComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.5f, 0.3f);
        menuRect.anchorMax = new Vector2(0.5f, 0.3f);
        menuRect.sizeDelta = new Vector2(500, 400);
        menuRect.anchoredPosition = Vector2.zero;

        // Create Buttons
        Button tutorialBtn = CreateMenuButton(menuPanel, "TutorialButton", "TUTORIAL", 0);
        Button bbqBtn = CreateMenuButton(menuPanel, "BBQButton", "OPERATION:\nBURNING BARBECUE", -100);
        Button cartBtn = CreateMenuButton(menuPanel, "ShoppingCartButton", "OPERATION:\nSHOPPING SCUFFLE", -200);
        Button exitBtn = CreateMenuButton(menuPanel, "ExitButton", "EXIT GAME", -300);

        // Style exit button differently
        ColorBlock exitColors = exitBtn.colors;
        exitColors.normalColor = new Color(0.8f, 0.2f, 0.2f);
        exitColors.highlightedColor = new Color(1f, 0.3f, 0.3f);
        exitBtn.colors = exitColors;

        // Add MainMenu component
        MainMenu mainMenu = canvasObj.AddComponent<MainMenu>();
        mainMenu.tutorialButton = tutorialBtn;
        mainMenu.bbqButton = bbqBtn;
        mainMenu.shoppingCartButton = cartBtn;
        mainMenu.exitButton = exitBtn;
        mainMenu.descriptionText = descText;

        Selection.activeGameObject = canvasObj;
        Debug.Log("Main Menu created successfully!");
    }

    static Button CreateMenuButton(GameObject parent, string name, string text, float yOffset)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 1f);
        buttonRect.anchorMax = new Vector2(0.5f, 1f);
        buttonRect.sizeDelta = new Vector2(450, 80);
        buttonRect.anchoredPosition = new Vector2(0, yOffset);

        // Button component
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.4f);

        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.4f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.5f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.3f);
        colors.selectedColor = new Color(0.35f, 0.35f, 0.45f);
        button.colors = colors;

        button.transition = Selectable.Transition.ColorTint;
        button.targetGraphic = buttonImage;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 32;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        return button;
    }
#endif
}