using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : MonoBehaviour
{
    public string questName;
    public InteractionCheckpoint[] interactionCheckpoints;

    public int lightValueToStartQuestLine;
    public bool spawnsNPC;
    public GameObject npcToSpawn;
    public Transform startPoint;

    public GameObject[] relatedNPCs;

    
    [SerializeField]
    private int interactionsCompleted;

    public event Action<QuestLine> questlineCompletedEvent;
    private bool questLineStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuestManager.instance.LightValueChanged += OnLightValueChanged;
        for(int i = 0; i < interactionCheckpoints.Length; i++)
        {
            interactionCheckpoints[i].interactionCompletedEvent += MarkInteractionComplete;
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
    

    // Update is called once per frame
}
