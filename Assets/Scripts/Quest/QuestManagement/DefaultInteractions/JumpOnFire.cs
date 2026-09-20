using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpOnFire : InteractionCheckpoint
{
    public Transform approachPoint;
    public Transform fire;

    public float walkSpeed = 1f;
    public float jumpSpeed = 6f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Interact()
    {
        StartCoroutine(JumpIn());
    }

    private IEnumerator JumpIn()
    {
        yield return StartCoroutine(MoveTo(approachPoint.position, walkSpeed));
        yield return StartCoroutine(MoveTo(fire.position, jumpSpeed));

        // this.GetComponent<SpriteRenderer>().enabled = false;
        MarkInteractionComplete();
        DisableGameObject();
    }

    private IEnumerator MoveTo(Vector2 target, float speed)
    {
        while ((target - rb.position).magnitude > 0.01f)
        {
            rb.MovePosition(
                Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime)
            );

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(target);
    }
    private void DisableGameObject()
    {
        this.gameObject.SetActive(false);
    }
}
