using UnityEngine;
using System;

public class Barbecue : MonoBehaviour
{
    [Header("---Team---")]
    public Team team = Team.Player;
    public string teamName = "Player Team";

    [Header("---Health---")]
    public float maxHP = 200f;
    public float currentHP = 200f;

    [Header("---UI---")]
    public BarbecueHealthUI healthUI;
    public Vector3 healthBarOffset = new Vector3(0f, 3f, 0); // Position above barbecue

    [Header("---Destruction---")]
    public GameObject destructionEffect; // Optional: particle effect
    public AudioClip destructionSound; // Optional: sound effect

    // Event for when barbecue is destroyed
    public event Action<Barbecue, Team> OnDestroyed;

    private bool isDestroyed = false;

    void Start()
    {
        currentHP = maxHP;

        // Create health bar UI if it doesn't exist
        if (healthUI == null)
        {
            CreateHealthUI();
        }
        else
        {
            // Health UI exists (manually created), just initialize it
            Debug.Log($"Using existing health UI for {teamName}");
        }

        UpdateHealthUI();
    }

    void Update()
    {
        // Test damage with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f, null);
        }
    }

    void CreateHealthUI()
    {
        // Find or create a canvas for world space UI
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("BarbecueCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        }

        // Create health UI GameObject
        GameObject healthUIObj = new GameObject($"{teamName}_HealthUI");
        healthUIObj.transform.SetParent(canvas.transform, false);
        healthUI = healthUIObj.AddComponent<BarbecueHealthUI>();
        healthUI.Initialize(this, healthBarOffset);
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        if (isDestroyed) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        Debug.Log($"{teamName} barbecue took {damage} damage! HP: {currentHP}/{maxHP}");

        UpdateHealthUI();

        if (currentHP <= 0f)
        {
            DestroyBarbecue(attacker);
        }
    }

    void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHP, maxHP);
        }
    }

    void DestroyBarbecue(GameObject attacker)
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // Determine which team won
        Team winningTeam = (team == Team.Player) ? Team.Enemy : Team.Player;

        Debug.Log($"{teamName} barbecue destroyed! {winningTeam} team wins!");

        // Trigger destruction event
        OnDestroyed?.Invoke(this, winningTeam);

        // Play effects
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        // Hide health UI
        if (healthUI != null)
        {
            healthUI.gameObject.SetActive(false);
        }

        // Destroy the barbecue
        gameObject.SetActive(false);
        // Or: Destroy(gameObject, 1f);
    }

    // Called when player or enemy collides during lunge
    void OnCollisionEnter(Collision collision)
    {
        // Check if hit by player lunge
        if (team == Team.Enemy && collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.GetComponent<PlayerController>() != null)
            {
                // Check if player is lunging (you may need to add a public isLunging getter)
                float damage = 25f; // Or get from player's weapon
                TakeDamage(damage, collision.gameObject);
            }
        }
        // Check if hit by enemy lunge
        else if (team == Team.Player && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null && !enemy.isDead)
            {
                // Enemy hit player's barbecue
                float damage = 20f; // Or get from enemy's weapon
                TakeDamage(damage, collision.gameObject);
            }
        }
    }
}

public enum Team
{
    Player,
    Enemy
}