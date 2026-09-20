using Unity.VisualScripting;
using UnityEngine;

public class ActivateQuestLineOnTriggerEnter : MonoBehaviour
{

    public QuestLine questLineToActivateOnTrigger;
    public Collider2D trigger;

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Trigger entered by: " + col.name);
        if(col.CompareTag("Player") && questLineToActivateOnTrigger.ValidateQuestLine())
        {
            Debug.Log("Activating quest line due to trigger: " + trigger.name);
            questLineToActivateOnTrigger.active = true;
        }
    }
}
