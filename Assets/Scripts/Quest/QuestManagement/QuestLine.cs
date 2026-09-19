using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : MonoBehaviour
{
    public string questName;
    public InteractionCheckpoint[] interactionCheckpoints;
    [SerializeField]
    private List<bool> interactionsCompleted;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionsCompleted = new List<bool>();

        QuestManager.instance.LightValueChanged += OnLightValueChanged;
        for(int i = 0; i < interactionCheckpoints.Length; i++)
        {
            interactionCheckpoints[i].questLine = this;
            interactionCheckpoints[i].interactionCompletedEvent += MarkInteractionComplete;
            interactionsCompleted.Add(false);
        }
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
        int index = Array.IndexOf(interactionCheckpoints, interactionCheckpoint);

        return interactionsCompleted[index];
    }

    private void MarkInteractionComplete(InteractionCheckpoint interactionCheckpoint)
    {
        int index = Array.IndexOf(interactionCheckpoints, interactionCheckpoint);
        Debug.Log("interaction " + (interactionsCompleted) + " have been completed");

        if(interactionsCompleted[index] == false)
        {
            MarkQuestlineComplete(index);
        }
        
    }

    private void MarkQuestlineComplete(int index)
    {
        interactionsCompleted[index] = true;
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
