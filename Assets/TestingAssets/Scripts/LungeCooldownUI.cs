using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LungeCooldownUI : MonoBehaviour
{
    [Header("---References---")]
    public PlayerController playerController;

    [Header("---UI Elements---")]
    public GameObject cooldownPanel;
    public Image cooldownFillImage;
    public TextMeshProUGUI cooldownText;

    [Header("---Colors---")]
    public Color cooldownColor = new Color(1f, 0.3f, 0.3f, 0.8f); // Red-ish
    public Color readyColor = new Color(0.3f, 1f, 0.3f, 0.8f); // Green-ish

    [Header("---Settings---")]
    public bool showWhenReady = false;

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (cooldownPanel == null)
        {
            cooldownPanel = this.gameObject;
        }

        // Add canvas group for fading
        canvasGroup = cooldownPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = cooldownPanel.AddComponent<CanvasGroup>();
        }

        // Start hidden
        //cooldownPanel.SetActive(false);
    }

    void Update()
    {
        if (playerController == null) return;

        // Access cooldown timer through reflection or make it public
        float cooldownRemaining = GetCooldownRemaining();
        float maxCooldown = playerController.lungeCooldown;

        if (cooldownRemaining > 0f)
        {
            // Show cooldown UI
            if (!cooldownPanel.activeSelf)
            {
                cooldownPanel.SetActive(true);
            }

            // Update fill amount (1 = full, 0 = empty)
            float fillAmount = cooldownRemaining / maxCooldown;
            if (cooldownFillImage != null)
            {
                cooldownFillImage.fillAmount = fillAmount;
                cooldownFillImage.color = cooldownColor;
            }

            // Update text
            if (cooldownText != null)
            {
                cooldownText.text = cooldownRemaining.ToString("F1") + "s";
                cooldownText.color = Color.white;
            }

            // Fade in
            canvasGroup.alpha = 1f;
        }
        else
        {
            // Cooldown complete
            if (showWhenReady)
            {
                // Show "Ready!" indicator
                if (cooldownFillImage != null)
                {
                    cooldownFillImage.fillAmount = 1f;
                    cooldownFillImage.color = readyColor;
                }

                if (cooldownText != null)
                {
                    cooldownText.text = "READY!";
                    cooldownText.color = readyColor;
                }

                // Fade out slowly
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * 2f);
            }
            else
            {
                // Hide completely when ready
                if (cooldownPanel.activeSelf)
                {
                    cooldownPanel.SetActive(false);
                }
            }
        }
    }

    float GetCooldownRemaining()
    {
        // Access private cooldownTimer through reflection
        var field = typeof(PlayerController).GetField("cooldownTimer",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (float)field.GetValue(playerController);
        }

        return 0f;
    }
}