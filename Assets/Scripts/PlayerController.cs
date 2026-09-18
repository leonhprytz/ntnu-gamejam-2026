using UnityEngine;
using UnityEngine.InputSystem;

enum MoveDirection
{
    Up,
    Right,
    Down,
    Left,
    None,
}

class MovePriority
{
    private int priority = 0;
    private int up,
        right,
        down,
        left = 0;

    public MovePriority() { }

    public void prioritize(MoveDirection md)
    {
        this.priority++;
        switch (md)
        {
            case MoveDirection.Up:
                this.up = this.priority;
                break;
            case MoveDirection.Right:
                this.right = this.priority;
                break;
            case MoveDirection.Down:
                this.down = this.priority;
                break;
            case MoveDirection.Left:
                this.left = this.priority;
                break;
        }
    }

    public void deprioritize(MoveDirection md)
    {
        switch (md)
        {
            case MoveDirection.Up:
                this.up = 0;
                break;
            case MoveDirection.Right:
                this.right = 0;
                break;
            case MoveDirection.Down:
                this.down = 0;
                break;
            case MoveDirection.Left:
                this.left = 0;
                break;
        }

        if (this.up == 0 && this.right == 0 && this.down == 0 && this.left == 0)
            this.priority = 0; // to avoid integer overflow lul
    }

    public MoveDirection getPriority()
    {
        int highestPriority = 0;
        MoveDirection output = MoveDirection.None;

        if (up > highestPriority)
        {
            highestPriority = up;
            output = MoveDirection.Up;
        }
        if (right > highestPriority)
        {
            highestPriority = right;
            output = MoveDirection.Right;
        }
        if (down > highestPriority)
        {
            highestPriority = down;
            output = MoveDirection.Down;
        }
        if (left > highestPriority)
        {
            highestPriority = left;
            output = MoveDirection.Left;
        }

        return output;
    }
}

public class PlayerController : MonoBehaviour
{
    private MovePriority movePriority = new MovePriority();

    public float moveSpeed;
    public Rigidbody2D rb;

    void handleKeyPress(Keyboard kb)
    {
        if (kb == null)
            return;

        if (kb.upArrowKey.wasPressedThisFrame)
        {
            this.movePriority.prioritize(MoveDirection.Up);
        }
        if (kb.upArrowKey.wasReleasedThisFrame)
        {
            this.movePriority.deprioritize(MoveDirection.Up);
        }

        if (kb.rightArrowKey.wasPressedThisFrame)
        {
            this.movePriority.prioritize(MoveDirection.Right);
        }
        if (kb.rightArrowKey.wasReleasedThisFrame)
        {
            this.movePriority.deprioritize(MoveDirection.Right);
        }

        if (kb.downArrowKey.wasPressedThisFrame)
        {
            this.movePriority.prioritize(MoveDirection.Down);
        }
        if (kb.downArrowKey.wasReleasedThisFrame)
        {
            this.movePriority.deprioritize(MoveDirection.Down);
        }

        if (kb.leftArrowKey.wasPressedThisFrame)
        {
            this.movePriority.prioritize(MoveDirection.Left);
        }
        if (kb.leftArrowKey.wasReleasedThisFrame)
        {
            this.movePriority.deprioritize(MoveDirection.Left);
        }
    }

    void move()
    {
        MoveDirection md = this.movePriority.getPriority();
        if (md == MoveDirection.None)
            return;

        Vector2 dv = Vector2.zero;
        switch (md)
        {
            case (MoveDirection.Up):
                dv = Vector2.up;
                break;
            case (MoveDirection.Right):
                dv = Vector2.right;
                break;
            case (MoveDirection.Down):
                dv = Vector2.down;
                break;
            case (MoveDirection.Left):
                dv = Vector2.left;
                break;
        }

        Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y);
        rb.MovePosition(pos + Time.deltaTime * dv * moveSpeed);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        handleKeyPress(Keyboard.current);
        move();
    }
}
