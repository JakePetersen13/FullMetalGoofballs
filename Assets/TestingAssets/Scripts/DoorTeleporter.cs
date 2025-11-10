using UnityEngine;
using System.Collections;

public class DoorTeleporter : MonoBehaviour
{
    [Header("---Teleport Settings---")]
    public Transform destinationDoor; // The door to teleport to
    public Vector3 teleportOffset = new Vector3(0, 0, 2f); // Offset from destination

    [Header("---Fade Settings---")]
    public float fadeOutDuration = 0.3f;
    public float fadeInDuration = 0.3f;
    public Color fadeColor = Color.black;

    [Header("---Optional---")]
    public bool requireKeyPress = false;
    public KeyCode interactKey = KeyCode.E;

    public bool playerInRange = false;
    public GameObject playerInTrigger = null;
    public bool isTeleporting = false;

    void Update()
    {
        // If require key press, check for input
        if (requireKeyPress && playerInRange && !isTeleporting)
        {
            if (Input.GetKeyDown(interactKey))
            {
                StartCoroutine(TeleportSequence(playerInTrigger));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            playerInRange = true;
            playerInTrigger = other.gameObject;

            // If don't require key press, teleport immediately
            if (!requireKeyPress)
            {
                StartCoroutine(TeleportSequence(other.gameObject));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInTrigger = null;
        }
    }

    IEnumerator TeleportSequence(GameObject player)
    {
        if (destinationDoor == null)
        {
            Debug.LogError("No destination door assigned!");
            yield break;
        }

        isTeleporting = true;

        // Fade out
        yield return StartCoroutine(FadeScreen(0f, 1f, fadeOutDuration));

        // Teleport player
        Vector3 destinationPos = destinationDoor.position + destinationDoor.forward * teleportOffset.z
                                                           + destinationDoor.right * teleportOffset.x
                                                           + destinationDoor.up * teleportOffset.y;
        player.transform.position = destinationPos;

        // Optional: Match player rotation to door
        Vector3 forwardDir = destinationDoor.forward;
        forwardDir.y = 0;
        if (forwardDir.sqrMagnitude > 0.01f)
        {
            player.transform.rotation = Quaternion.LookRotation(forwardDir);
        }

        // Small delay at black screen
        yield return new WaitForSeconds(0.1f);

        // Fade in
        yield return StartCoroutine(FadeScreen(1f, 0f, fadeInDuration));

        isTeleporting = false;
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        // Get or create fade overlay
        FadeOverlay overlay = FadeOverlay.Instance;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            overlay.SetAlpha(alpha);
            yield return null;
        }

        overlay.SetAlpha(endAlpha);
    }

    // Visual debug in editor
    void OnDrawGizmos()
    {
        if (destinationDoor != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationDoor.position);

            Vector3 destinationPos = destinationDoor.position + destinationDoor.forward * teleportOffset.z
                                                               + destinationDoor.right * teleportOffset.x
                                                               + destinationDoor.up * teleportOffset.y;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destinationPos, 0.5f);
        }
    }
}