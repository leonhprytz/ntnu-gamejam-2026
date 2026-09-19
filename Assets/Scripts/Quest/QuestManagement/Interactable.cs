using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{

    void OnEnable()
    {
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.InteractWasPressed += TryInteract;
        }
    }

    void OnDisable()
    {
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.InteractWasPressed -= TryInteract;
        }
    }

    protected abstract void TryInteract();
}
