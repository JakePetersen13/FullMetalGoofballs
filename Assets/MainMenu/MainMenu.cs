using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("---Menu Buttons---")]
    public Button tutorialButton;
    public Button bbqButton;
    public Button shoppingCartButton;
    public Button exitButton;

    [Header("---Description Text---")]
    public TextMeshProUGUI descriptionText;
    public string defaultDescription = "SELECT A MODE";

    [Header("---Button Descriptions---")]
    [TextArea(2, 4)]
    public string tutorialDescription = "Learn the basics of combat and movement";
    [TextArea(2, 4)]
    public string bbqDescription = "Defend your BBQ and destroy the enemy's grill!";
    [TextArea(2, 4)]
    public string shoppingCartDescription = "Shopping chaos! Protect your cart from rivals!";
    [TextArea(2, 4)]
    public string exitDescription = "Exit to desktop";

    [Header("---Scene Names---")]
    public string tutorialSceneName = "Tutorial";
    public string bbqSceneName = "BBQ_Level";
    public string shoppingCartSceneName = "ShoppingCart_Level";

    [Header("---Audio---")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip hoverSound;

    void Start()
    {
        // Setup button listeners
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(() => LoadScene(tutorialSceneName));
            AddHoverEvents(tutorialButton, tutorialDescription);
        }

        if (bbqButton != null)
        {
            bbqButton.onClick.AddListener(() => LoadScene(bbqSceneName));
            AddHoverEvents(bbqButton, bbqDescription);
        }

        if (shoppingCartButton != null)
        {
            shoppingCartButton.onClick.AddListener(() => LoadScene(shoppingCartSceneName));
            AddHoverEvents(shoppingCartButton, shoppingCartDescription);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            AddHoverEvents(exitButton, exitDescription);
        }

        // Setup audio source
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set default description
        if (descriptionText != null)
        {
            descriptionText.text = defaultDescription;
        }
    }

    void AddHoverEvents(Button button, string description)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Hover enter
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { OnButtonHover(description); });
        trigger.triggers.Add(enterEntry);

        // Hover exit
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnButtonExit(); });
        trigger.triggers.Add(exitEntry);
    }

    void OnButtonHover(string description)
    {
        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void OnButtonExit()
    {
        if (descriptionText != null)
        {
            descriptionText.text = defaultDescription;
        }
    }

    void LoadScene(string sceneName)
    {
        PlayButtonSound();
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    void ExitGame()
    {
        PlayButtonSound();
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}