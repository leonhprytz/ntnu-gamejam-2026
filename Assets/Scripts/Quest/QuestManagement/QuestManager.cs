using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public QuestLine[] questlines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < questlines.Length; i++)
        {
            questlines[i].questlineCompletedEvent += QuestlineCompleted;
        }        
    }

    void QuestlineCompleted(QuestLine questline)
    {
        Debug.Log("Questline: " + questline.questName + " has been completed");
    }


}
