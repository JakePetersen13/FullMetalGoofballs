using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("---References---")]
    public PlayerController playerController;

    [Header("---UI Elements---")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Image fillImage;
    public GameObject healthBarPanel; // The entire health bar UI

    [Header("---Colors---")]
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.25f; // 25% HP
    public float midHealthThreshold = 0.5f;  // 50% HP

    [Header("---Animation---")]
    public bool animateHealthChange = true;
    public float lerpSpeed = 5f;

    [Header("---Visibility---")]
    public bool isVisible = true;

    public float targetHealth;
    public float displayedHealth;
    public float healthPercent = 1f;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerHealthUI: No PlayerController found!");
                return;
            }
        }

        // If healthBarPanel not assigned, use this gameObject
        if (healthBarPanel == null)
        {
            healthBarPanel = this.gameObject;
        }

        // Initialize
        targetHealth = playerController.HP;
        displayedHealth = targetHealth;
        UpdateHealthUI();
        SetVisible(isVisible);
    }

    void Update()
    {
        if (playerController == null) return;

        if (targetHealth == playerController.maxHP)
            targetHealth = playerController.HP;
        else
            targetHealth = playerController.HP - 1;

        // Smooth health bar animation
        if (animateHealthChange)
        {
            displayedHealth = Mathf.Lerp(displayedHealth, targetHealth, Time.deltaTime * lerpSpeed);
        }
        else
        {
            displayedHealth = targetHealth;
        }

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        float maxHealth = playerController.maxHP;
        healthPercent = displayedHealth / maxHealth;

        // Update slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = displayedHealth;
        }

        // Update text
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(displayedHealth)} / {maxHealth}";
        }

        // Update color based on health percentage
        if (fillImage != null)
        {
            if (healthPercent <= lowHealthThreshold)
            {
                fillImage.color = lowHealthColor;
            }
            else if (healthPercent <= midHealthThreshold)
            {
                fillImage.color = Color.Lerp(lowHealthColor, midHealthColor,
                    (healthPercent - lowHealthThreshold) / (midHealthThreshold - lowHealthThreshold));
            }
            else
            {
                fillImage.color = Color.Lerp(midHealthColor, fullHealthColor,
                    (healthPercent - midHealthThreshold) / (1f - midHealthThreshold));
            }
        }
    }

    // Public methods for TutorialManager to control visibility
    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (healthBarPanel != null)
        {
            healthBarPanel.SetActive(visible);
        }
    }

    public void Show()
    {
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

}