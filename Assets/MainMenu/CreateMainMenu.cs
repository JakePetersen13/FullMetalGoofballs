using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateMainMenu : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Goofy Military Main Menu", false, 14)]
    static void CreateMainMenuUI()
    {
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.08f, 0.08f, 0.12f);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "FULL METAL\nGOOFBALLS™";
        titleText.fontSize = 84;
        titleText.color = new Color(1f, 0.85f, 0.25f);
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.75f);
        titleRect.anchorMax = new Vector2(0.5f, 0.75f);
        titleRect.sizeDelta = new Vector2(900, 250);

        GameObject stampObj = new GameObject("ClassificationStamp");
        stampObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI stampText = stampObj.AddComponent<TextMeshProUGUI>();
        stampText.text = "? TOP SECRET ?\nAUTHORIZED PERSONNEL ONLY";
        stampText.fontSize = 22;
        stampText.color = new Color(1f, 0.2f, 0.2f);
        stampText.alignment = TextAlignmentOptions.Center;
        stampText.fontStyle = FontStyles.Bold | FontStyles.Italic;

        RectTransform stampRect = stampObj.GetComponent<RectTransform>();
        stampRect.anchorMin = new Vector2(0.5f, 0.63f);
        stampRect.anchorMax = new Vector2(0.5f, 0.63f);
        stampRect.sizeDelta = new Vector2(700, 80);

        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "SERVICE GUARANTEES NOTHING";
        descText.fontSize = 30;
        descText.color = new Color(0.9f, 0.9f, 1f);
        descText.alignment = TextAlignmentOptions.Center;
        descText.fontStyle = FontStyles.Italic;

        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.55f);
        descRect.anchorMax = new Vector2(0.5f, 0.55f);
        descRect.sizeDelta = new Vector2(1000, 120);

        GameObject menuPanel = new GameObject("MenuPanel");
        menuPanel.transform.SetParent(canvas.transform, false);

        RectTransform menuRect = menuPanel.AddComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.5f, 0.3f);
        menuRect.anchorMax = new Vector2(0.5f, 0.3f);
        menuRect.sizeDelta = new Vector2(520, 420);

        Button tutorialBtn = CreateMenuButton(menuPanel, "TutorialButton",
            "BASIC TRAINING\n(NO SURVIVORS)", 0);

        Button bbqBtn = CreateMenuButton(menuPanel, "BBQButton",
            "OPERATION:\nCHARRED DOMINANCE", -100);

        Button cartBtn = CreateMenuButton(menuPanel, "ShoppingCartButton",
            "OPERATION:\nTACTICAL GROCERY", -200);

        Button exitBtn = CreateMenuButton(menuPanel, "ExitButton",
            "ABANDON POST", -300);

        ColorBlock exitColors = exitBtn.colors;
        exitColors.normalColor = new Color(0.8f, 0.2f, 0.2f);
        exitColors.highlightedColor = new Color(1f, 0.3f, 0.3f);
        exitBtn.colors = exitColors;

        MainMenu mainMenu = canvasObj.AddComponent<MainMenu>();
        mainMenu.tutorialButton = tutorialBtn;
        mainMenu.bbqButton = bbqBtn;
        mainMenu.shoppingCartButton = cartBtn;
        mainMenu.exitButton = exitBtn;
        mainMenu.descriptionText = descText;

        Selection.activeGameObject = canvasObj;
        Debug.Log("Goofy Military Main Menu created!");
    }

    static Button CreateMenuButton(GameObject parent, string name, string text, float yOffset)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(460, 82);
        rect.anchoredPosition = new Vector2(0, yOffset);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.4f);

        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.4f);
        colors.highlightedColor = new Color(0.45f, 0.45f, 0.55f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.3f);
        btn.colors = colors;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 32;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.Center;
        txt.fontStyle = FontStyles.Bold;

        RectTransform txtRect = txt.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = new Vector2(10, 10);
        txtRect.offsetMax = new Vector2(-10, -10);

        buttonObj.AddComponent<ButtonJitter>();
        return btn;
    }
#endif
}
