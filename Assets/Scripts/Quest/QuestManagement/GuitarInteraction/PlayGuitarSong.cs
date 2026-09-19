using System.Collections;
using UnityEngine;

// Sends an NPC over to its spot, plays the song there once, and only then
// completes the step.
public class PlayGuitarSong : InteractionCheckpoint
{
    [Tooltip("Set its WhenToPlay to onCue: this step drives the movement itself.")]
    public NPCMovement movement;

    [Tooltip("Optional: found on this object when left empty.")]
    public AudioSource audioSource;
    public AudioClip song;

    private bool playing;

    public override void Interact()
    {
        // The manager is held busy for the whole sequence, but a branch can
        // still drive this step again, so guard it here too.
        if (playing)
        {
            return;
        }

        playing = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        SetBusy(true);

        if (movement != null)
        {
            yield return StartCoroutine(movement.PlayMovement());
        }

        yield return StartCoroutine(PlaySong());

        SetBusy(false);
        playing = false;
        MarkInteractionComplete();
    }

    private IEnumerator PlaySong()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            Debug.LogWarning(name + " has no AudioSource; the song is skipped.");
            yield break;
        }

        if (song != null)
        {
            audioSource.clip = song;
        }

        if (audioSource.clip == null)
        {
            Debug.LogWarning(name + " has no song assigned; the step completes right away.");
            yield break;
        }

        // Looping would never hand control back, so one pass is forced here
        // rather than relying on how the source is set up in the inspector.
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitWhile(() => audioSource.isPlaying);
    }

    private void SetBusy(bool busy)
    {
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.IsBusy = busy;
        }
    }
}
