using System;
using UnityEngine;

public class MelodyResponseQuest : InteractionCheckpoint
{
    public override void TryInteract()
    {
        if (VerifyInteraction())
        {
            Debug.Log("MelodyResponseQuest");   
            MarkInteractionComplete();
        }
        
    }
}