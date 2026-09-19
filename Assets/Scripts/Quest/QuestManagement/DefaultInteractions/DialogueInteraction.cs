using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Driven by the QuestLine that owns it, one line per interact press. Nest the
// Dialogue prefab under the NPC and this hides itself until a step plays.
public class DialogueInteraction : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.03f;

    private Canvas canvas;
    private InputAction interactAction;

    private string[] lines;
    private int index;
    private Action onFinished;
    private Coroutine typeRoutine;
    private bool typing;
    private int lineStartFrame;

    // Set from the first line until the last one is fully on screen. The
    // questline routes interact presses to Advance while this is set.
    public bool IsPlaying { get; private set; }

    void Awake()
    {
        // The component can sit on the box itself, so toggle the canvas rather
        // than the GameObject: a disabled GameObject can't run the typing
        // coroutine that has to bring it back.
        canvas = GetComponentInParent<Canvas>(true);
        interactAction = InputSystem.actions.FindAction("Interact");
        Hide();
    }

    void Update()
    {
        // A press mid-type snaps the line to full. The InteractionManager is
        // busy while typing, so this is the only thing that sees that press.
        if (!typing || interactAction == null)
        {
            return;
        }

        // Ignore the very press that started this line: the manager handles it
        // earlier in the same frame, and it would skip the line instantly.
        if (Time.frameCount != lineStartFrame && interactAction.WasPressedThisFrame())
        {
            FinishLine();
        }
    }

    public void Play(string[] dialogueLines, Action finishedCallback)
    {
        onFinished = finishedCallback;

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Finish();
            return;
        }

        lines = dialogueLines;
        index = 0;
        IsPlaying = true;

        if (canvas != null)
        {
            canvas.enabled = true;
        }

        StartLine();
    }

    // One press, one line.
    public void Advance()
    {
        if (!IsPlaying || typing || index >= lines.Length - 1)
        {
            return;
        }

        index++;
        StartLine();
    }

    public void Hide()
    {
        StopTyping();
        IsPlaying = false;
        onFinished = null;

        if (textComponent != null)
        {
            textComponent.text = string.Empty;
        }

        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    void StartLine()
    {
        StopTyping();
        textComponent.text = string.Empty;
        typing = true;
        lineStartFrame = Time.frameCount;
        SetBusy(true);
        typeRoutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        typeRoutine = null;
        FinishLine();
    }

    void FinishLine()
    {
        StopTyping();
        textComponent.text = lines[index];

        // The step is done once its last line is on screen. The box stays up so
        // the player can read it; the next step clears it.
        if (index >= lines.Length - 1)
        {
            Finish();
        }
    }

    void Finish()
    {
        IsPlaying = false;

        Action callback = onFinished;
        onFinished = null;
        callback?.Invoke();
    }

    void StopTyping()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }

        typing = false;
        SetBusy(false);
    }

    void SetBusy(bool busy)
    {
        if (InteractionManager.instance != null)
        {
            InteractionManager.instance.IsBusy = busy;
        }
    }
}
