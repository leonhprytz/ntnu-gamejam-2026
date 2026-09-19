using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCMovement : MonoBehaviour
{
    private MoveDirection _md;
    private MoveDirection animationMoveDirection
    {
        get { return _md; }
        set
        {
            bool update = _md != value;
            if (!update)
                return;
            _md = value;

            Animator animator = GetComponent<Animator>();
            animator?.SetInteger("moveDirection", (int)animationMoveDirection);
        }
    }

    public enum WhenToPlay
    {
        onStart,
        afterInteraction,
        // Doesn't start on its own: an interaction runs PlayMovement and waits
        // for it, so it can do something else once the NPC has arrived.
        onCue
    }

    public float movementSpeed;
    public Transform[] movementPoints;

    public WhenToPlay whenToPlay;

    public InteractionCheckpoint interactionCheckpoint;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        if (whenToPlay == WhenToPlay.onStart)
        {
            StartCoroutine(PlayMovement());
        }
        else if(whenToPlay == WhenToPlay.afterInteraction)
        {
            interactionCheckpoint.interactionCompletedEvent += StartMovement;
        }
    }

    private void StartMovement()
    {
        StartCoroutine(PlayMovement());
    }

    private void StartMovement(InteractionCheckpoint interactionCheckpoint)
    {
        StartCoroutine(PlayMovement());
    }
    public IEnumerator PlayMovement()
    {
        for (int i = 0; i < movementPoints.Length; i++)
        {
            yield return StartCoroutine(GoToPoint(i));
        }
    }

    private MoveDirection calculateAnimationMoveDirection(Vector2 vector)
    {
        MoveDirection best = MoveDirection.None;
        float score = 0;

        float dot;
        if ((dot = Vector2.Dot(vector, Vector2.up)) > score)
        {
            best = MoveDirection.Up;
            score = dot;
        }
        if ((dot = Vector2.Dot(vector, Vector2.right)) > score)
        {
            best = MoveDirection.Right;
            score = dot;
        }
        if ((dot = Vector2.Dot(vector, Vector2.down)) > score)
        {
            best = MoveDirection.Down;
            score = dot;
        }
        if ((dot = Vector2.Dot(vector, Vector2.left)) > score)
        {
            best = MoveDirection.Left;
            score = dot;
        }

        return best;
    }

    private IEnumerator GoToPoint(int index)
    {
        Vector2 point = movementPoints[index].position;

        animationMoveDirection = calculateAnimationMoveDirection(point - rb.position);
        while ((point - rb.position).magnitude > 0.1)
        {
            Vector2 movementVector = point - rb.position;
            Vector2 moveDir = movementVector.normalized * movementSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveDir);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(point);
        animationMoveDirection = MoveDirection.None;
    }
}
