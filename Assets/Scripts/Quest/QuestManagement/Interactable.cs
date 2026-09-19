using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Per-instance tiebreak, added on top of the type's base priority.")]
    public int priority;

    // Questlines override this so they always outrank scenery.
    public virtual int BasePriority => 0;

    public int Priority => BasePriority + priority;

    // Returns true if this interactable consumed the press. Returning false lets
    // the InteractionManager fall through to the next candidate in range.
    public abstract bool TryInteract();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        InteractionManager.instance.addAvailableInteraction(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        InteractionManager.instance.removeAvailableInteraction(this);
    }

    void OnDisable()
    {
        // OnTriggerExit2D never fires when a GameObject is disabled or destroyed,
        // so without this a despawned NPC stays interactable from across the map.
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.removeAvailableInteraction(this);
        }
    }
}
