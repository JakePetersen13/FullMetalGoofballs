using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("---Barbecues---")]
    public Barbecue playerBarbecue;
    public Barbecue enemyBarbecue;

    [Header("---Victory Screens---")]
    public GameObject redVictoryScreen;  // Shows when red team (enemy) wins
    public GameObject blueVictoryScreen; // Shows when blue team (player) wins

    [Header("---Settings---")]
    public float victoryDelay = 2f;
    public bool autoRestartOnVictory = false;
    public float restartDelay = 5f;

    private bool gameEnded = false;

    void Start()
    {
        // Subscribe to barbecue destruction events
        if (playerBarbecue != null)
        {
            playerBarbecue.OnDestroyed += HandleBarbecueDestroyed;
        }

        if (enemyBarbecue != null)
        {
            enemyBarbecue.OnDestroyed += HandleBarbecueDestroyed;
        }

        // Hide victory screens at start
        if (redVictoryScreen != null)
        {
            redVictoryScreen.SetActive(false);
        }

        if (blueVictoryScreen != null)
        {
            blueVictoryScreen.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerBarbecue != null)
        {
            playerBarbecue.OnDestroyed -= HandleBarbecueDestroyed;
        }

        if (enemyBarbecue != null)
        {
            enemyBarbecue.OnDestroyed -= HandleBarbecueDestroyed;
        }
    }

    void HandleBarbecueDestroyed(Barbecue destroyedBarbecue, Team winningTeam)
    {
        if (gameEnded) return;

        gameEnded = true;

        // Hide all health UIs
        HideAllHealthUIs();

        // Check which barbecue was destroyed
        if (destroyedBarbecue == playerBarbecue)
        {
            // Player barbecue destroyed = Blue team (enemy) wins
            Debug.Log("Player barbecue destroyed! Blue Team (Enemy) Wins!");
            StartCoroutine(SlowDownAndShowVictory(blueVictoryScreen));
        }
        else if (destroyedBarbecue == enemyBarbecue)
        {
            // Enemy barbecue destroyed = Red team (player) wins
            Debug.Log("Enemy barbecue destroyed! Red Team (Player) Wins!");
            StartCoroutine(SlowDownAndShowVictory(redVictoryScreen));
        }
    }

    void HideAllHealthUIs()
    {
        // Hide player health UI
        PlayerHealthUI playerHealthUI = FindObjectOfType<PlayerHealthUI>();
        if (playerHealthUI != null)
        {
            playerHealthUI.Hide();
        }

        // Hide barbecue health UIs
        if (playerBarbecue != null && playerBarbecue.healthUI != null)
        {
            playerBarbecue.healthUI.gameObject.SetActive(false);
        }

        if (enemyBarbecue != null && enemyBarbecue.healthUI != null)
        {
            enemyBarbecue.healthUI.gameObject.SetActive(false);
        }
    }

    IEnumerator SlowDownAndShowVictory(GameObject victoryScreen)
    {
        // Get fade overlay
        FadeOverlay fadeOverlay = FadeOverlay.Instance;

        // Gradually slow down time and fade to black over victoryDelay duration
        float elapsed = 0f;
        float startTimeScale = Time.timeScale;

        while (elapsed < victoryDelay)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / victoryDelay;

            // Smooth slow-motion curve
            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, t);

            // Fade to black at the same time
            fadeOverlay.SetAlpha(Mathf.Lerp(0f, 1f, t));

            yield return null;
        }

        // Ensure fully paused and black
        Time.timeScale = 0f;
        fadeOverlay.SetAlpha(1f);

        // Small delay at black screen
        yield return new WaitForSecondsRealtime(0.3f);

        // Show victory screen
        yield return StartCoroutine(ShowVictoryScreen(victoryScreen));

        // Fade in from black to reveal victory screen
        float fadeInDuration = 0.5f;
        elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeInDuration;
            fadeOverlay.SetAlpha(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        fadeOverlay.SetAlpha(0f);
    }

    IEnumerator ShowVictoryScreen(GameObject victoryScreen)
    {
        // Show appropriate victory screen
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Victory screen is not assigned!");
        }

        // Auto restart if enabled
        if (autoRestartOnVictory)
        {
            yield return new WaitForSecondsRealtime(restartDelay);
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // Reset time scale before restarting
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    // Public method to check game state
    public bool IsGameEnded()
    {
        return gameEnded;
    }

    // Get remaining HP of barbecues
    public float GetPlayerBarbecueHP()
    {
        return playerBarbecue != null ? playerBarbecue.currentHP : 0f;
    }

    public float GetEnemyBarbecueHP()
    {
        return enemyBarbecue != null ? enemyBarbecue.currentHP : 0f;
    }
}