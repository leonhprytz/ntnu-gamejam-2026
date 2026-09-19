using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InteractionManager : MonoBehaviour
{

    public static InteractionManager instance = null;

    private InputAction interactAction;

    public event Action InteractWasPressed;

    void Awake()
    {
        if (InteractionManager.instance == null)
        {
            InteractionManager.instance = this;
        }

        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            InteractWasPressed?.Invoke();
        }
    }
}
