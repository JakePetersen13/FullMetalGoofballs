using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    public DoorTeleporter door1;
    public DoorTeleporter door2;
    public PlayerController playerController;
    public EnemyController enemy1;
    public EnemyController enemy2;
    public EnemyController enemy3;
    public EnemyController enemy4;
    public DoorTeleporter hallwayTrigger;
    public PlayerHealthUI playerHealthUI; // Add this reference
    public int kills = 0;

    [Range(1, 8)]
    public int progressCounter = 1;

    [Header("---Audio---")]
    public AudioPlayerHelper audioPlayer;
    public AudioSource audioSource;
    public AudioClip reville;
    public AudioClip barracksDialouge1;
    public AudioClip barracksDialouge2;
    public AudioClip barracksDialouge3;
    public AudioClip hallwayDialouge1;
    public AudioClip hallwayDialouge2;
    public AudioClip hallwayDialouge3;
    public AudioClip arenaDialouge1;
    public AudioClip arenaDialouge2;
    public AudioClip arenaDialouge3;
    public AudioClip nullAudio = null;

    void Start()
    {
        // Find health UI if not assigned
        if (playerHealthUI == null)
        {
            playerHealthUI = FindObjectOfType<PlayerHealthUI>();
        }

        // Start with health bar hidden
        if (playerHealthUI != null)
        {
            playerHealthUI.Hide();
        }
    }

    void Update()
    {
        switch (progressCounter)
        {
            case 1:
                disableMovement(4f + barracksDialouge1.length + barracksDialouge2.length + barracksDialouge3.length);
                audioPlayer.PlayOneShot(reville, 0.15f);
                StartCoroutine(audioWaitThenPlayInOrder(3f, barracksDialouge1, barracksDialouge2, barracksDialouge3));
                progressCounter++;
                break;
            case 2:
                if (door1.playerTouched) 
                    progressCounter++;
                break;
            case 3:
                playerController.rb.velocity = Vector3.zero;
                disableMovement(hallwayDialouge1.length);
                audioPlayer.PlayOneShot(hallwayDialouge1);
                progressCounter++;
                break;
            case 4:
                if (hallwayTrigger.playerTouched)
                {
                    playerController.rb.velocity = Vector3.zero;
                    disableMovement(1f + hallwayDialouge2.length + hallwayDialouge3.length);
                    StartCoroutine(audioWaitThenPlayInOrder(0f, hallwayDialouge2, hallwayDialouge3, nullAudio));
                    progressCounter++;
                }
                break;
            case 5:
                if (door2.playerTouched)
                    progressCounter++;
                break;
            case 6:
                playerController.rb.velocity = Vector3.zero;
                disableMovement(arenaDialouge1.length);
                audioPlayer.PlayOneShot(arenaDialouge1);
                progressCounter++;
                break;
            case 7:
                if (enemy1 == null)
                {
                    playerController.rb.velocity = Vector3.zero;
                    disableMovement(arenaDialouge2.length);
                    audioPlayer.PlayOneShot(arenaDialouge2);
                    StartCoroutine(waitToEnemySpawn(arenaDialouge2.length / 2f));
                    progressCounter++;
                }
                break;
            case 8:
                StartCoroutine(waitToActivateEnemy(arenaDialouge2.length / 2f));
                if ( enemy2 == null && enemy3 == null && enemy4 == null)
                {
                    audioPlayer.PlayOneShot(arenaDialouge3);
                    waitToEndScene(arenaDialouge3.length + 1f);
                }
                break;
        }
    }

    void disableMovement(float seconds)
    {
        playerController.canMove = false;
        StartCoroutine(wait(seconds));
    }

    IEnumerator wait(float seconds)
    {
        Debug.Log("Wait started at: " + Time.time);
        yield return new WaitForSeconds(seconds);
        Debug.Log("Wait ended: " + Time.time);
        playerController.canMove = true;
    }

    IEnumerator waitToEnemySpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemy2.transform.position = new Vector3(13.1f, 2f, -171.826f);
        enemy3.transform.position = new Vector3(13.1f, 2f, -173.826f);
        enemy4.transform.position = new Vector3(13.1f, 2f, -168.826f);
    }

    IEnumerator waitToActivateEnemy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        enemy2.canLunge = true;
        enemy3.canLunge = true;
        enemy4.canLunge = true;
    }

    IEnumerator waitToEndScene(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        FadeOverlay fadeOverlay = FadeOverlay.Instance;
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeOverlay.SetAlpha(alpha);
            yield return null;
        }
        fadeOverlay.SetAlpha(1f);

        // Restart scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    IEnumerator audioWaitThenPlayInOrder(float seconds, AudioClip clip1, AudioClip clip2, AudioClip clip3)
    {
        yield return new WaitForSeconds(seconds);
        if (clip1 != null)
        {
            audioPlayer.PlayOneShot(clip1);
            yield return new WaitForSeconds(clip1.length + 0.5f);
        }
        if (clip2 != null)
        {
            audioPlayer.PlayOneShot(clip2);
            yield return new WaitForSeconds(clip2.length + 0.5f);
        }
        if (clip3 != null)
        {
            audioPlayer.PlayOneShot(clip3);
            yield return new WaitForSeconds(clip3.length + 0.5f);
        }
    }
}