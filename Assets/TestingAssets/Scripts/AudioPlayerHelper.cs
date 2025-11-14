using UnityEngine;
using System.Collections;

public class AudioPlayerHelper : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // Auto-find AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    // Play audio and wait for it to finish
    public IEnumerator PlayAndWait(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audio clip provided!");
            yield break;
        }

        audioSource.clip = clip;
        audioSource.Play();

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);
    }

    // Play audio and wait, with custom volume
    public IEnumerator PlayAndWait(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audio clip provided!");
            yield break;
        }

        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();

        yield return new WaitForSeconds(clip.length);
    }

    // Play audio without waiting (fire and forget)
    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Play audio without waiting, with custom volume
    public void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    // Stop current audio
    public void Stop()
    {
        audioSource.Stop();
    }

    // Check if audio is currently playing
    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}