using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : Interactable
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

    // Questlines outrank scenery, so walking up to an NPC standing next to a
    // tree always talks to the NPC.
    public override int BasePriority => 100;


    void Start()
    {
        // interactionsCompleted = new List<bool>();

        QuestManager.instance.LightValueChanged += OnLightValueChanged;
        for(int i = 0; i < interactionCheckpoints.Length; i++)
        {
            interactionCheckpoints[i].setQuestLine(this);
            interactionCheckpoints[i].interactionCompletedEvent += MarkInteractionComplete;
            // interactionsCompleted.Add(false);
        }

        // LightValueChanged only fires on a change, so a questline whose
        // threshold is already met at boot would otherwise never start.
        OnLightValueChanged(QuestManager.instance.lightValue);
    }

    public override bool TryInteract()
    {
        // Decline the press rather than swallowing it, so a finished or
        // not-yet-available questline doesn't block whatever else is in range.
        if (!questLineStarted) return false;
        if (currentInteractionCheckpointIndex >= interactionCheckpoints.Length) return false;

        interactionCheckpoints[currentInteractionCheckpointIndex].Interact();
        return true;
    }
    public void StartQuestLine()
    {
        Debug.Log("questline started");
        if(npcToSpawn != null)
        {
            npcToSpawn.SetActive(true);
        }
    }

    private void MarkInteractionComplete(InteractionCheckpoint interactionCheckpoint)
    {
        // Only the step the questline is currently on may advance the cursor.
        // TryInteract can no longer drive the wrong step, but a checkpoint can
        // still self-complete via its public interactionCompleted flag.
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
