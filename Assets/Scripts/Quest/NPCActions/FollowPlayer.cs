using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowPlayer : InteractionCheckpoint
{
    public Interactable interactableToActivateOnEnd;

    public float movementSpeed;
    public float followDistance = 1f;

    [Tooltip("Following stops once this NPC is inside this trigger.")]
    public Collider2D stopTrigger;

    public NPCMovement.WhenToPlay whenToPlay;

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (whenToPlay == NPCMovement.WhenToPlay.onStart)
        {
            StartCoroutine(Follow());
        }
    }

    public override void Interact()
    {
        StartCoroutine(Follow());
    }

    public IEnumerator Follow()
    {
        while (!stopTrigger.OverlapPoint(rb.position))
        {
            Vector2 movementVector = (Vector2)player.position - rb.position;

            if (movementVector.magnitude > followDistance)
            {
                Vector2 moveDir = movementVector.normalized * movementSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + moveDir);
            }

            yield return new WaitForFixedUpdate();
        }

        if (interactableToActivateOnEnd)
        {
            interactableToActivateOnEnd.active = true;
        }
        MarkInteractionComplete();
    }
}
