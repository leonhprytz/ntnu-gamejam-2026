using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public static QuestManager instance = null;
    public QuestLine[] questlines;
    public List<bool> questlinesCompleted;

    public int lightValue;
    private int prevLightValue;

    public event Action<int> LightValueChanged;

    public BonfireManager bonfireManager;


    void OnEnable()
    {
        if(QuestManager.instance == null)
        {
            QuestManager.instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questlinesCompleted = new List<bool>();
        prevLightValue = lightValue;

        for(int i = 0; i < questlines.Length; i++)
        {
            questlines[i].questlineCompletedEvent += QuestlineCompleted;
            questlinesCompleted.Add(false);
        }        
    }

    void FixedUpdate()
    {
        if(lightValue != prevLightValue)
        {
            LightValueChanged?.Invoke(lightValue);
        }        
        prevLightValue = lightValue;
    }

    void QuestlineCompleted(QuestLine questline)
    {
        Debug.Log("Questline: " + questline.questName + " has been completed");
        if (questline.questLineWon)
        {
            lightValue += questline.lightValueWin;
        }
        else
        {
            lightValue -= questline.lightValueLose;
        }
        bonfireManager.increaseBonfireSize();
    }

}
