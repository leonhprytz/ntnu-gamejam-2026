using System;
using UnityEngine;

public class InteractionCheckpoint : MonoBehaviour
{

    public event Action<InteractionCheckpoint> interactionCompletedEvent;
    public bool interactionCompleted;
    
    public QuestLine questLine;

    private bool done = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public bool VerifyInteraction()
    {
        questLine.VerifyInteraction(this);
        return true;        
    }

    public void MarkInteractionComplete()
    {
        interactionCompletedEvent?.Invoke(this);
    }

    public void SetQuestLineWon(bool win)
    {
        questLine.questLineWon = win;
    }

    void Update()
    {
        if (done)
        {
            return;
        }

        if (interactionCompleted)
        {
            done = true;
            MarkInteractionComplete();
        }
    }
    
}

