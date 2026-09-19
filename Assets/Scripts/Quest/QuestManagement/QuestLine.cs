using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : MonoBehaviour
{
    public string questName;
    public InteractionCheckpoint[] interactionCheckpoints;

    private int currentInteractionCheckpointIndex = 0;

    [Header("light value settings")]
    public int lightValueToStartQuestLine;
    public int lightValueWin;
    public int lightValueLose;

    [Header("npc settings")]
    public bool spawnsNPC;
    public GameObject npcToSpawn;
    public Transform startPoint;


    

    public event Action<QuestLine> questlineCompletedEvent;
    private bool questLineStarted = false;
    public bool questLineWon;

    void Start()
    {
        InteractionManager.instance.InteractWasPressed += tryInteractionCheckpoint;
        // interactionsCompleted = new List<bool>();

        QuestManager.instance.LightValueChanged += OnLightValueChanged;
        for(int i = 0; i < interactionCheckpoints.Length; i++)
        {
            interactionCheckpoints[i].setQuestLine(this);
            interactionCheckpoints[i].interactionCompletedEvent += MarkInteractionComplete;
            // interactionsCompleted.Add(false);
        }
    }

    public void tryInteractionCheckpoint()
    {
        if (currentInteractionCheckpointIndex >= interactionCheckpoints.Length) return;
        interactionCheckpoints[currentInteractionCheckpointIndex].TryInteract();
    }
    public void StartQuestLine()
    {
        Debug.Log("questline started");
        if(npcToSpawn != null)
        {
            npcToSpawn.SetActive(true);
        }
    }

    public bool VerifyInteraction(InteractionCheckpoint interactionCheckpoint){
        // int index = Array.IndexOf(interactionCheckpoints, interactionCheckpoint);

        // return interactionsCompleted[index];
        return currentInteractionCheckpointIndex == Array.IndexOf(interactionCheckpoints, interactionCheckpoint);

    }

    private void MarkInteractionComplete(InteractionCheckpoint interactionCheckpoint)
    {
        // Only the step the questline is currently on may advance the cursor.
        if (Array.IndexOf(interactionCheckpoints, interactionCheckpoint) != currentInteractionCheckpointIndex)
        {
            return;
        }

        Debug.Log("interaction " + interactionCheckpoint.name + " have been completed");
        currentInteractionCheckpointIndex ++;

        // if(interactionsCompleted[index] == false)
        // {
        //     MarkQuestlineComplete(index);
        // }
        if (currentInteractionCheckpointIndex == interactionCheckpoints.Length)
        {
            MarkQuestlineComplete();
        }
        
    }

    private void MarkQuestlineComplete()
    {
        // interactionsCompleted[index] = true;
        questlineCompletedEvent?.Invoke(this);
    }


    void OnLightValueChanged(int currentLightValue)
    {
        if (questLineStarted)
        {
            return;
        }

        if(currentLightValue >= lightValueToStartQuestLine)
        {
            StartQuestLine();
            questLineStarted = true;
        }
    }
    
}
