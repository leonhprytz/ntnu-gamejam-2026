using System;
using UnityEngine;

// A single step in a QuestLine. Not an Interactable: the questline is what the
// player interacts with, and it drives its current checkpoint directly.
public abstract class InteractionCheckpoint : MonoBehaviour
{
    public event Action<InteractionCheckpoint> interactionCompletedEvent;
    public bool interactionCompleted;

    private QuestLine questLine;

    private bool done = false;

    public void setQuestLine(QuestLine questLine)
    {
        this.questLine = questLine;
    }

    public abstract void Interact();

    public void MarkInteractionComplete()
    {
        if (done)
        {
            return;
        }

        done = true;
        interactionCompleted = true;
        interactionCompletedEvent?.Invoke(this);
    }

    void Update()
    {
        if (!done && interactionCompleted)
        {
            MarkInteractionComplete();
        }
    }
}
