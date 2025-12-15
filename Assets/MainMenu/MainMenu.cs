using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("---Buttons---")]
    public Button tutorialButton;
    public Button bbqButton;
    public Button shoppingCartButton;
    public Button exitButton;

    [Header("---Description---")]
    public TextMeshProUGUI descriptionText;

    [Header("---Scenes---")]
    public string tutorialSceneName = "Tutorial";
    public string bbqSceneName = "BBQ";
    public string shoppingCartSceneName = "ShoppingCart_Level";

    [Header("---Audio---")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip hoverSound;

    string[] slogans =
    {
        "SERVICE GUARANTEES NOTHING",
        "PROUDLY UNTESTED",
        "QUALITY IS OPTIONAL",
        "BUDGET WAS CUT",
        "NOW WITH 30% MORE FREEDOM"
    };

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        descriptionText.text = slogans[Random.Range(0, slogans.Length)];

        SetupButton(tutorialButton,
            "FAILURE IS NOT PERMITTED.\nYOU WILL LEARN. QUICKLY.",
            () => LoadScene(tutorialSceneName));

        SetupButton(bbqButton,
            "DEFEND THE GRILL.\nTHE MEAT MUST FLOW.",
            () => LoadScene(bbqSceneName));

        SetupButton(shoppingCartButton,
            "URBAN WARFARE.\nAISLE SEVEN IS HOSTILE.",
            () => LoadScene(shoppingCartSceneName));

        SetupButton(exitButton,
            "DESERTION WILL BE NOTED.",
            ExitGame);
    }

    void SetupButton(Button button, string hoverText, UnityEngine.Events.UnityAction clickAction)
    {
        if (button == null) return;

        ButtonJitter jitter = button.GetComponent<ButtonJitter>();
        button.onClick.AddListener(clickAction);

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter.callback.AddListener((_) =>
        {
            descriptionText.text = hoverText;
            descriptionText.color = Random.value > 0.5f
                ? new Color(1f, 0.25f, 0.25f)
                : new Color(1f, 0.9f, 0.3f);

            jitter?.StartJitter();
            audioSource?.PlayOneShot(hoverSound);
        });

        EventTrigger.Entry exit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit.callback.AddListener((_) =>
        {
            descriptionText.text = slogans[Random.Range(0, slogans.Length)];
            descriptionText.color = Color.white;
            jitter?.StopJitter();
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    void LoadScene(string scene)
    {
        audioSource?.PlayOneShot(buttonClickSound);
        SceneManager.LoadScene(scene);
    }

    void ExitGame()
    {
        audioSource?.PlayOneShot(buttonClickSound);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
