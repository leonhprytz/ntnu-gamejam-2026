using System;
using UnityEngine;

public abstract class InteractionCheckpoint : MonoBehaviour
{

    public event Action<InteractionCheckpoint> interactionCompletedEvent;
    public bool interactionCompleted;
    
    private QuestLine questLine;

    private bool done = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void setQuestLine(QuestLine questLine)
    {
        this.questLine = questLine;
    }

    public abstract void TryInteract();

    public bool VerifyInteraction()
    {
        return questLine.VerifyInteraction(this);        
    }

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

    public void SetQuestLineWon(bool win)
    {
        questLine.questLineWon = win;
    }

    void Update()
    {
        if (!done && interactionCompleted)
        {
            MarkInteractionComplete();
        }
    }
    
}

