using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinousMovement : MonoBehaviour
{
    public QuestLine questLine;

    public List<Transform> waypoints;
    public float moveSpeed = 1f;
    private Rigidbody2D rb;

    private Transform nextWaypoint;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(MoveContinuously());
    }
    private void OnDisable()
    {
        StopCoroutine(MoveContinuously());
    }
    private IEnumerator MoveContinuously()
    {
        int index = 0;
        // looping between waypoints 
        while (!questLine.hasInteracted)
        {
            nextWaypoint = waypoints[index];

            // moving towards next waypoint
            while ((nextWaypoint.transform.position - transform.position).magnitude > 0.01f && !questLine.hasInteracted)
            {
                Vector2 moveDir = (nextWaypoint.position - transform.position).normalized;
                rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDir);
                yield return new WaitForFixedUpdate();
            }

            index = (index + 1) % waypoints.Count;
            yield return null;
        }
    }
}
