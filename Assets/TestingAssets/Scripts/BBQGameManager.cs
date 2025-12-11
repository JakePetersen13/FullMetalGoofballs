using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BBQGameManager : MonoBehaviour
{
    [Header("---Barbecues---")]
    public Barbecue playerBarbecue;
    public Barbecue enemyBarbecue;

    [Header("---Victory UI---")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;

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

        // Hide victory panel at start
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
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

        Debug.Log($"Game Over! {winningTeam} team wins!");

        StartCoroutine(ShowVictoryScreen(winningTeam));
    }

    IEnumerator ShowVictoryScreen(Team winningTeam)
    {
        // Wait a moment before showing victory
        yield return new WaitForSeconds(victoryDelay);

        // Show victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Update victory text
        if (victoryText != null)
        {
            if (winningTeam == Team.Player)
            {
                victoryText.text = "VICTORY!\nPlayer Team Wins!";
                victoryText.color = Color.green;
            }
            else
            {
                victoryText.text = "DEFEAT!\nEnemy Team Wins!";
                victoryText.color = Color.red;
            }
        }

        // Auto restart if enabled
        if (autoRestartOnVictory)
        {
            yield return new WaitForSeconds(restartDelay);
            RestartGame();
        }
    }

    public void RestartGame()
    {
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