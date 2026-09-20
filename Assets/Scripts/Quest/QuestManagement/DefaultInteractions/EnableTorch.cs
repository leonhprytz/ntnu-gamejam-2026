using UnityEngine;

public class EnableTorch : InteractionCheckpoint
{
    public override void Interact()
    {
        Debug.Log("Enabling torch!!");
        PlayerController playerController = GetComponent<PlayerController>();
        playerController.torchUnlocked = true;
        MarkInteractionComplete();
    }
}
