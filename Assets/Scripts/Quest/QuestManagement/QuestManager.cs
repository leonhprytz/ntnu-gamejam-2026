using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public static QuestManager instance;
    public QuestLine[] questlines;

    public int lightValue;
    private int prevLightValue;

    public event Action<int> LightValueChanged;
    void onAwake()
    {
        if(QuestManager.instance == null)
        {
            QuestManager.instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < questlines.Length; i++)
        {
            questlines[i].questlineCompletedEvent += QuestlineCompleted;
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
    }

}
