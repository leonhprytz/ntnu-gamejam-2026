using System;
using UnityEngine;

public class PlayGuitarQuest : InteractionCheckpoint
{
    public override void Interact()
    {
        Debug.Log("PlayGuitarQuest interacted");
        MarkInteractionComplete();
    }
}
