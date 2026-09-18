using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLine : MonoBehaviour
{
    public string questName;
    public InteractionCheckpoint[] interactionCheckpoints;

    
    [SerializeField]
    private int interactionsCompleted;

    public event Action<QuestLine> questlineCompletedEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < interactionCheckpoints.Length; i++)
        {
            interactionCheckpoints[i].interactionCompletedEvent += MarkInteractionComplete;
        }
    }

    private void MarkInteractionComplete(InteractionCheckpoint interactionCheckpoint)
    {
        interactionsCompleted++;
        Debug.Log("interaction " + (interactionsCompleted) + " have been completed");

        if(interactionsCompleted >= interactionCheckpoints.Length)
        {
            MarkQuestlineComplete();
        }
    }

    private void MarkQuestlineComplete()
    {
        questlineCompletedEvent?.Invoke(this);
    }

    

    // Update is called once per frame
}
