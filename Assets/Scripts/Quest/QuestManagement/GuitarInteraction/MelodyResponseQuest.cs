using System;
using UnityEngine;

public class MelodyResponseQuest : InteractionCheckpoint
{
    public override void Interact()
    {
        Debug.Log("MelodyResponseQuest");
        MarkInteractionComplete();
    }
}