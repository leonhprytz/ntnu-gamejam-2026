using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCMovement : MonoBehaviour
{
    public enum WhenToPlay
    {
        onStart,
        afterInteraction
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

        if(whenToPlay == WhenToPlay.onStart)
        {
            StartCoroutine(PlayMovement());
        }
        else
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
    private IEnumerator PlayMovement()
    {
        for(int i = 0; i < movementPoints.Length; i++)
        {
            yield return StartCoroutine(GoToPoint(i));
        }


    }

    private IEnumerator GoToPoint(int index)
    {
        Vector2 point = movementPoints[index].position;

        while( (point - rb.position).magnitude > 0.1)
        {

            Vector2 movementVector = point - rb.position;
            Vector2 moveDir = movementVector.normalized * 
                movementSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(rb.position + moveDir);

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(point);
    }

}
