using UnityEngine;

public class ThrowWaterOnFire : InteractionCheckpoint
{
    private BonfireManager bonfire;

    void Awake()
    {
        // == null, not ??=: that also catches a reference whose object is gone.
        if (bonfire == null)
        {
            bonfire = FindFirstObjectByType<BonfireManager>();
        }
    }

    public override void Interact()
    {
        bonfire.AddLight(-30);
        MarkInteractionComplete();
    }
}
