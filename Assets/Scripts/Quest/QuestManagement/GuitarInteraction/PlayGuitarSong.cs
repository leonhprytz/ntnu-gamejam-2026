using System.Collections;
using UnityEngine;

// Sends an NPC over to its spot, plays the song there once, and only then
// completes the step.
public class PlayGuitarSong : InteractionCheckpoint
{
    [Tooltip("Optional: found on this object when left empty.")]
    public AudioSource audioSource;

    private bool playing;

    public override void Interact()
    {
        PlaySong();
    }

    void PlaySong()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
            return;
        if (audioSource.clip == null)
            return;

        audioSource.Play();
        MarkInteractionComplete();
    }

    private void SetBusy(bool busy)
    {
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.IsBusy = busy;
        }
    }
}
