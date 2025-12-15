using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("---AI Spawning---")]
    public GameObject playerAIPrefab;
    public GameObject enemyAIPrefab;
    public Transform playerAISpawnPoint;
    public Transform enemyAISpawnPoint;
    public float spawnInterval = 1.5f;

    [Header("---Player Respawn---")]
    public GameObject playerObject;
    public Transform playerSpawnPoint;
    public float playerRespawnDelay = 2f;

    private const int maxPlayerAI = 3;
    private const int maxEnemyAI = 4;
    private List<PlayerAI> activePlayerAIs = new List<PlayerAI>();
    private List<EnemyController> activeEnemies = new List<EnemyController>();
    private float lastPlayerAISpawnTime = 0f;
    private float lastEnemySpawnTime = 0f;
    private bool gameEnded = false;
    private bool isRespawningPlayer = false;

    [Header("---Game Start---")]
    public TextMeshProUGUI countdownText;
    public AudioSource countdownAudioSource;
    public AudioClip countdown;
    public AudioClip music;
    public float countdownInterval = 1f;

    public AudioClip redWins;
    public AudioClip blueWins;
    public AudioClip explode;

    void Start()
    {
        Time.timeScale = 0f;

        FadeOverlay.Instance.SetAlpha(1f); // full black

        StartCoroutine(GameStartSequence());
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

        // Initialize spawn timers to allow immediate first spawn
        lastPlayerAISpawnTime = -spawnInterval;
        lastEnemySpawnTime = -spawnInterval;
    }

    void Update()
    {
        if (gameEnded) return;

        // Check if player needs respawning
        if (!isRespawningPlayer && playerObject != null && !playerObject.activeInHierarchy)
        {
            StartCoroutine(RespawnPlayer());
        }

        // Clean up null references and check for inactive AI
        activePlayerAIs.RemoveAll(ai => ai == null);
        activeEnemies.RemoveAll(enemy => enemy == null);

        // Check for inactive PlayerAI and respawn them
        for (int i = activePlayerAIs.Count - 1; i >= 0; i--)
        {
            if (activePlayerAIs[i] != null && !activePlayerAIs[i].gameObject.activeInHierarchy)
            {
                Destroy(activePlayerAIs[i]);
                activePlayerAIs.RemoveAt(i);
            }
        }

        // Check for inactive Enemies and respawn them
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null && !activeEnemies[i].gameObject.activeInHierarchy)
            {
                Destroy(activeEnemies[i]);
                activeEnemies.RemoveAt(i);
            }
        }

        // Check and spawn PlayerAI if needed
        if (activePlayerAIs.Count < maxPlayerAI && Time.time >= lastPlayerAISpawnTime + spawnInterval)
        {
            SpawnPlayerAI();
            lastPlayerAISpawnTime = Time.time;
        }

        // Check and spawn Enemy if needed
        if (activeEnemies.Count < maxEnemyAI && Time.time >= lastEnemySpawnTime + spawnInterval)
        {
            SpawnEnemy();
            lastEnemySpawnTime = Time.time;
        }
    }

    void SpawnPlayerAI()
    {
        if (playerAIPrefab == null || playerAISpawnPoint == null)
        {
            Debug.LogWarning("PlayerAI prefab or spawn point not assigned!");
            return;
        }

        GameObject aiObj = Instantiate(playerAIPrefab, playerAISpawnPoint.position, playerAISpawnPoint.rotation);
        PlayerAI playerAI = aiObj.GetComponent<PlayerAI>();

        if (playerAI != null)
        {
            // Set target barbecue for the AI
            if (enemyBarbecue != null)
            {
                playerAI.targetBarbecue = enemyBarbecue;
            }

            activePlayerAIs.Add(playerAI);
            Debug.Log($"Spawned PlayerAI. Active count: {activePlayerAIs.Count}/{maxPlayerAI}");
        }
        else
        {
            Debug.LogError("PlayerAI component not found on spawned prefab!");
        }
    }

    void SpawnEnemy()
    {
        if (enemyAIPrefab == null || enemyAISpawnPoint == null)
        {
            Debug.LogWarning("Enemy prefab or spawn point not assigned!");
            return;
        }

        GameObject enemyObj = Instantiate(enemyAIPrefab, enemyAISpawnPoint.position, enemyAISpawnPoint.rotation);
        EnemyController enemy = enemyObj.GetComponent<EnemyController>();

        if (enemy != null)
        {
            // Set target barbecue for the enemy
            if (playerBarbecue != null)
            {
                enemy.targetBarbecue = playerBarbecue;
            }

            activeEnemies.Add(enemy);
            Debug.Log($"Spawned Enemy. Active count: {activeEnemies.Count}/{maxEnemyAI}");
        }
        else
        {
            Debug.LogError("EnemyController component not found on spawned prefab!");
        }
    }

    IEnumerator RespawnPlayer()
    {
        isRespawningPlayer = true;
        playerObject.GetComponent<PlayerController>().isDead = false;

        // Wait for respawn delay
        yield return new WaitForSeconds(playerRespawnDelay);

        if (playerObject != null && playerSpawnPoint != null)
        {
            // Move player to spawn point
            playerObject.transform.position = playerSpawnPoint.position;
            playerObject.transform.rotation = playerSpawnPoint.rotation;

            // Reset player velocity if they have a Rigidbody
            Rigidbody rb = playerObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerObject.GetComponent<PlayerController>().HP = playerObject.GetComponent<PlayerController>().maxHP;
            playerObject.GetComponent<PlayerController>().canMove = true;
            // Reactivate the player
            playerObject.SetActive(true);

            Debug.Log("Player respawned!");
            playerObject.GetComponent<PlayerController>().damageFlashUI.SetTransparent();
        }
        else
        {
            Debug.LogWarning("Player object or spawn point not assigned!");
        }

        isRespawningPlayer = false;
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
            countdownAudioSource.Stop();
            countdownAudioSource.loop = false;
            countdownAudioSource.volume = 1f;
            countdownAudioSource.PlayOneShot(explode);
            StartCoroutine(SlowDownAndShowVictory(blueVictoryScreen));

        }
        else if (destroyedBarbecue == enemyBarbecue)
        {
            // Enemy barbecue destroyed = Red team (player) wins
            Debug.Log("Enemy barbecue destroyed! Red Team (Player) Wins!");
            countdownAudioSource.Stop();
            countdownAudioSource.loop = false;
            countdownAudioSource.volume = 1f;
            countdownAudioSource.PlayOneShot(explode);
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

        if (victoryScreen == redVictoryScreen)
            countdownAudioSource.PlayOneShot(redWins);
        else
            countdownAudioSource.PlayOneShot(blueWins);

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
    IEnumerator GameStartSequence()
    {
        yield return new WaitForSecondsRealtime(1f);

        countdownAudioSource.PlayOneShot(countdown);
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();


            yield return new WaitForSecondsRealtime(countdownInterval);
        }

        // GO!
        countdownText.text = "GO!";

        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);

        // Fade in & unpause
        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            FadeOverlay.Instance.SetAlpha(Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        FadeOverlay.Instance.SetAlpha(0f);
        Time.timeScale = 1f;
        countdownAudioSource.volume = 0.25f;
        countdownAudioSource.loop = true;
        countdownAudioSource.clip = music;
        countdownAudioSource.Play();
    }


}