using System;
using UnityEngine;

public class InteractionCheckpoint : MonoBehaviour
{

    public event Action<InteractionCheckpoint> interactionCompletedEvent;
    public bool interactionCompleted;

    private bool done = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void MarkInteractionComplete()
    {
        interactionCompletedEvent?.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            return;
        }

        if (interactionCompleted)
        {
            done = true;
            MarkInteractionComplete();
        }
    }
    
}
