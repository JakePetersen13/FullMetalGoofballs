using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BBQHealthHUD : MonoBehaviour
{
    [Header("---References---")]
    public Barbecue playerBarbecue;
    public Barbecue enemyBarbecue;

    [Header("---UI Elements---")]
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public Image playerFillImage;

    public Slider enemyHealthSlider;
    public TextMeshProUGUI enemyHealthText;
    public Image enemyFillImage;

    [Header("---Colors---")]
    public Color playerColor = new Color(0.2f, 0.6f, 1f); // Blue
    public Color enemyColor = new Color(1f, 0.3f, 0.2f); // Red

    void Start()
    {
        // Auto-find barbecues if not assigned
        if (playerBarbecue == null || enemyBarbecue == null)
        {
            Barbecue[] allBarbecues = FindObjectsOfType<Barbecue>();
            foreach (Barbecue bbq in allBarbecues)
            {
                if (bbq.team == Team.Player && playerBarbecue == null)
                {
                    playerBarbecue = bbq;
                }
                else if (bbq.team == Team.Enemy && enemyBarbecue == null)
                {
                    enemyBarbecue = bbq;
                }
            }
        }

        // Set colors
        if (playerFillImage != null)
        {
            playerFillImage.color = playerColor;
        }

        if (enemyFillImage != null)
        {
            enemyFillImage.color = enemyColor;
        }

        UpdateHealthBars();
    }

    void Update()
    {
        UpdateHealthBars();
    }

    void UpdateHealthBars()
    {
        // Update player barbecue health
        if (playerBarbecue != null)
        {
            float healthPercent = playerBarbecue.currentHP / playerBarbecue.maxHP;

            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = healthPercent;
            }

            if (playerHealthText != null)
            {
                playerHealthText.text = $"{Mathf.CeilToInt(playerBarbecue.currentHP)} / {playerBarbecue.maxHP}";
            }

            if (playerFillImage != null)
            {
                playerFillImage.fillAmount = healthPercent;
            }
        }

        // Update enemy barbecue health
        if (enemyBarbecue != null)
        {
            float healthPercent = enemyBarbecue.currentHP / enemyBarbecue.maxHP;

            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.value = healthPercent;
            }

            if (enemyHealthText != null)
            {
                enemyHealthText.text = $"{Mathf.CeilToInt(enemyBarbecue.currentHP)} / {enemyBarbecue.maxHP}";
            }

            if (enemyFillImage != null)
            {
                enemyFillImage.fillAmount = healthPercent;
            }
        }
    }
}