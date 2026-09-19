using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{

    public static InteractionManager instance = null;

    // Used to sort candidates by distance. This manager is its own GameObject,
    // not the player, so its own transform is no help here.
    public Transform player;

    private InputAction interactAction;

    // Set by an interaction that runs over several frames (dialogue, minigames)
    // so mashing the interact button can't restart it.
    public bool IsBusy;

    private readonly List<Interactable> availableInteractions = new List<Interactable>();

    void Awake()
    {
        if (InteractionManager.instance == null)
        {
            InteractionManager.instance = this;
        }

        interactAction = InputSystem.actions.FindAction("Interact");

        if (player == null)
        {
            Debug.LogWarning("InteractionManager has no player assigned; interactions in range will be ordered by priority only.");
        }
    }

    void Update()
    {
        if (IsBusy)
        {
            return;
        }

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            playPrioritizedInteraction();
        }
    }

    public void addAvailableInteraction(Interactable interaction)
    {
        // Overlapping colliders can raise enter twice for the same interactable.
        if (interaction == null || availableInteractions.Contains(interaction))
        {
            return;
        }

        availableInteractions.Add(interaction);
    }

    public void removeAvailableInteraction(Interactable interaction)
    {
        availableInteractions.Remove(interaction);
    }

    public void playPrioritizedInteraction()
    {
        availableInteractions.RemoveAll(i => i == null);

        if (availableInteractions.Count == 0)
        {
            return;
        }

        availableInteractions.Sort(ComparePriority);

        // Walk the candidates until one accepts. A questline that is finished (or
        // not started yet) declines, so the tree behind it is still reachable.
        for (int i = 0; i < availableInteractions.Count; i++)
        {
            if (availableInteractions[i].TryInteract())
            {
                return;
            }
        }
    }

    private int ComparePriority(Interactable a, Interactable b)
    {
        // Highest priority first, nearest first within the same priority.
        int byPriority = b.Priority.CompareTo(a.Priority);
        if (byPriority != 0)
        {
            return byPriority;
        }

        return SqrDistanceToPlayer(a).CompareTo(SqrDistanceToPlayer(b));
    }

    private float SqrDistanceToPlayer(Interactable interactable)
    {
        if (player == null)
        {
            return 0f;
        }

        return ((Vector2)interactable.transform.position - (Vector2)player.position).sqrMagnitude;
    }

}
