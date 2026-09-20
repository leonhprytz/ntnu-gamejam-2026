using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance = null;
    public QuestLine[] questlines;
    public List<bool> questlinesCompleted;

    public float lightValue
    {
        get { return bonfireManager.lightAmount; }
        set
        {
            Debug.Log("Set light amount to: " + value);
            bonfireManager.lightAmount = value;
        }
    }

    private float prevLightValue;
    public event Action<float> LightValueChanged;

    public BonfireManager bonfireManager;

    void OnEnable()
    {
        if (QuestManager.instance == null)
        {
            QuestManager.instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questlinesCompleted = new List<bool>();
        prevLightValue = lightValue;

        for (int i = 0; i < questlines.Length; i++)
        {
            questlines[i].questlineCompletedEvent += QuestlineCompleted;
            questlinesCompleted.Add(false);
        }
    }

    void FixedUpdate()
    {
        if (lightValue != prevLightValue)
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
    }

    public bool CheckIfQuestLineIsValid(QuestLine questline)
    {
        if (questline == null)
        {
            Debug.LogWarning("Questline is null");
            return false;
        }

        if (lightValue >= questline.lightValueToStartQuestLine)
        {
            return true;
        }

        return false;
    }
}
