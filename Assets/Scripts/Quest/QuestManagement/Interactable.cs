using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Per-instance tiebreak, added on top of the type's base priority.")]
    public int priority;

    private bool _isInRange = false;
    public bool isInRange
    {
        get => _isInRange;
        set
        {
            _isInRange = value;
            if (!active)
                return;

            if (isInRange)
            {
                InteractionManager.instance.addAvailableInteraction(this);
            }
            else
            {
                InteractionManager.instance.removeAvailableInteraction(this);
            }
        }
    }

    [SerializeField]
    private bool _active;
    public bool active
    {
        get => _active;
        set
        {
            _active = value;
            if (!active)
            {
                InteractionManager.instance.removeAvailableInteraction(this);
            }
            else if (isInRange)
            {
                InteractionManager.instance.addAvailableInteraction(this);
            }
        }
    }

    // Questlines override this so they always outrank scenery.
    public virtual int BasePriority => 0;

    public int Priority => BasePriority + priority;

    // Returns true if this interactable consumed the press. Returning false lets
    // the InteractionManager fall through to the next candidate in range.
    public abstract bool TryInteract();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isInRange = true;
        Debug.Log("In range " + isInRange);
        Debug.Log("Active " + active);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isInRange = false;
        Debug.Log("In range " + isInRange);
        Debug.Log("Active " + active);
    }

    void OnValidate()
    {
        active = _active;
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
