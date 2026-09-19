using System;
using UnityEngine;

public class PlayGuitarQuest : InteractionCheckpoint
{
    public override void TryInteract()
    {
        if (VerifyInteraction())
        {
            Debug.Log("PlayGuitarQuest interacted");
            MarkInteractionComplete();
        }
    }
}
