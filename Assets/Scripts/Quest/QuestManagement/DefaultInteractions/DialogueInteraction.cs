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
    private string[] choices;
    private Action<int> onChosen;
    private Keyboard typedChoiceKeyboard;
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
        // While alternatives are up the box waits for a number key instead; the
        // interact button is left alone so it can't pick one by accident.
        if (choices != null)
        {
            PollChoice();
            return;
        }

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

    // Called by the questline when the finished step branches. Keeps IsPlaying
    // set so interact presses stay with the box until an alternative is picked.
    public void AskChoice(string[] options, Action<int> chosenCallback)
    {
        choices = options;
        onChosen = chosenCallback;
        IsPlaying = true;
        ListenForTypedChoice();

        if (canvas != null)
        {
            canvas.enabled = true;
        }

        for (int i = 0; i < options.Length; i++)
        {
            textComponent.text += "\n" + (i + 1) + ") " + options[i];
        }
    }

    public void Hide()
    {
        StopTyping();
        StopListeningForTypedChoice();
        IsPlaying = false;
        onFinished = null;
        choices = null;
        onChosen = null;

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

    void PollChoice()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        for (int i = 0; i < choices.Length && i < 9; i++)
        {
            if (keyboard[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame)
            {
                Choose(i);
                return;
            }
        }
    }

    void Choose(int index)
    {
        if (choices == null)
        {
            return;
        }

        choices = null;
        StopListeningForTypedChoice();

        Action<int> callback = onChosen;
        onChosen = null;
        IsPlaying = false;
        callback?.Invoke(index);
    }

    // The Key enum addresses physical key positions, which WebGL can't resolve
    // for digits, so the typed character is what actually arrives in a build.
    void ListenForTypedChoice()
    {
        if (typedChoiceKeyboard != null || Keyboard.current == null)
        {
            return;
        }

        typedChoiceKeyboard = Keyboard.current;
        typedChoiceKeyboard.onTextInput += OnTextInput;
    }

    void StopListeningForTypedChoice()
    {
        if (typedChoiceKeyboard == null)
        {
            return;
        }

        typedChoiceKeyboard.onTextInput -= OnTextInput;
        typedChoiceKeyboard = null;
    }

    void OnTextInput(char character)
    {
        if (choices == null)
        {
            return;
        }

        int index = character - '1';

        if (index < 0 || index >= choices.Length)
        {
            return;
        }

        Choose(index);
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
