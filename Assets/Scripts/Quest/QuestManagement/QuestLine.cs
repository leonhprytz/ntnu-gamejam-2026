using System;
using System.Linq;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    Started,
    Won,
    Loss,
}

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : Interactable
{
    public string questName;
    public QuestStep[] steps;

    private bool firstStep = true;

    [SerializeField]
    private QuestState _state = QuestState.NotStarted;
    public QuestState state
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (state)
            {
                case QuestState.Started:
                    StartQuestLine();
                    break;
                case QuestState.Won:
                    Win();
                    break;
                case QuestState.Loss:
                    Lose();
                    break;
            }
        }
    }

    [Tooltip("Optional: found among this NPC's children when left empty.")]
    public DialogueInteraction dialogue;

    private int currentStepIndex = 0;

    [Header("light value settings")]
    public float lightValueToStartQuestLine;
    public float lightValueWin;
    public float lightValueLose;

    [Header("npc settings")]
    public bool spawnsNPC;
    public GameObject npcToSpawn;

    public event Action<QuestLine> questlineCompletedEvent;
    private bool questLineStarted
    {
        get { return state == QuestState.Started; }
    }

    [SerializeField]
    public bool questLineWon;

    // Questlines outrank scenery, so walking up to an NPC standing next to a
    // tree always talks to the NPC.
    public override int BasePriority => 100;

    void Start()
    {
        if (dialogue == null)
        {
            dialogue = GetComponentInChildren<DialogueInteraction>(true);
        }

        QuestManager.instance.LightValueChanged += OnLightValueChanged;
        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i].IsDialogue || steps[i].logic == null)
            {
                continue;
            }

            steps[i].logic.setQuestLine(this);
            steps[i].logic.interactionCompletedEvent += MarkInteractionComplete;
        }

        // LightValueChanged only fires on a change, so a questline whose
        // threshold is already met at boot would otherwise never start.
        OnLightValueChanged(QuestManager.instance.lightValue);
    }

    public override bool TryInteract()
    {
        // Decline the press rather than swallowing it, so a finished or
        // not-yet-available questline doesn't block whatever else is in range.
        if (!questLineStarted)
            return false;

        // Mid-conversation the press means "next line", not "next step".
        if (dialogue != null && dialogue.IsPlaying)
        {
            dialogue.Advance();
            return true;
        }

        if (currentStepIndex >= steps.Length)
        {
            // Finished, but the closing line is still up: dismiss it and still
            // decline, so whatever else is in range gets the press next time.
            if (dialogue != null)
            {
                dialogue.Hide();
            }

            return false;
        }

        Debug.Log("Current step: " + currentStepIndex);
        QuestStep step = steps[currentStepIndex];

        if (step.IsDialogue)
        {
            if (dialogue == null)
            {
                Debug.LogWarning(
                    questName + " has a dialogue step but no DialogueInteraction in its children."
                );
                return false;
            }

            dialogue.Play(step.lines, CompleteCurrentStep);
            return true;
        }

        if (step.logic == null)
            return false;

        // Clear the last step's line so it isn't left hanging over the logic.
        if (dialogue != null)
        {
            dialogue.Hide();
        }

        // step is an actione, they return when the step is done on their own
        step.logic.Interact();
        return true;
    }

    public void StartQuestLine()
    {
        Debug.Log("Questline " + this.name + " started");
        Debug.Log("Started as " + this.active);
        GoToStep(0);
        if (npcToSpawn != null)
        {
            npcToSpawn.SetActive(true);
        }
    }

    // checks if the questline is valid and can be started
    public bool ValidateQuestLine()
    {
        return QuestManager.instance.CheckIfQuestLineIsValid(this);
    }

    private void MarkInteractionComplete(InteractionCheckpoint interactionCheckpoint)
    {
        // Only the step the questline is currently on may advance the cursor.
        // TryInteract can no longer drive the wrong step, but a checkpoint can
        // still self-complete via its public interactionCompleted flag.
        if (currentStepIndex >= steps.Length)
            return;
        if (steps[currentStepIndex].logic != interactionCheckpoint)
            return;

        Debug.Log("interaction " + interactionCheckpoint.name + " have been completed");
        CompleteCurrentStep();
    }

    // Shared by both step kinds: logic steps arrive here via their completion
    // event, dialogue steps via the callback fired on their last line.
    private void CompleteCurrentStep()
    {
        QuestStep step = currentStepIndex < steps.Length ? steps[currentStepIndex] : null;

        if (step == null)
        {
            state = QuestState.Won;
        }

        // A branching step doesn't advance on its own: the box puts the
        // alternatives up and hands back the one the player picked.
        if (step != null && step.HasBranches && dialogue != null)
        {
            QuestBranch[] branches = step.branches;
            dialogue.AskChoice(
                branches.Select(b => b.option).ToArray(),
                // Picking an alternative counts as the press that plays the
                // step it leads to, so the box doesn't sit there waiting.
                chosen =>
                {
                    GoToStep(branches[chosen].targetStep);
                }
            );
            return;
        }

        GoToStep(currentStepIndex + 1);
    }

    private void GoToStep(int stepIndex)
    {
        QuestStep step = currentStepIndex < steps.Length ? steps[currentStepIndex] : null;
        if (step != null && !firstStep)
        {
            switch (step.nextState)
            {
                case StateAfterCompletion.Win:
                    state = QuestState.Won;
                    return;
                case StateAfterCompletion.Lose:
                    state = QuestState.Loss;
                    return;
                case StateAfterCompletion.Continue:
                    break; // just follow through
            }
        }
        firstStep = false;

        currentStepIndex = Mathf.Clamp(stepIndex, 0, steps.Length);

        if (currentStepIndex < steps.Length)
        {
            if (this.steps[currentStepIndex].auto)
            {
                TryInteract();
            }
            return;
        }

        // assume win if end of quest is reached without explicit state change
        state = QuestState.Won;
    }

    private void Win()
    {
        active = false;
        QuestManager.instance.lightValue += lightValueWin;
        // The box isn't hidden here: a closing dialogue step should stay
        // readable until the player presses interact again.
        questlineCompletedEvent?.Invoke(this);
    }

    private void Lose()
    {
        active = false;
        QuestManager.instance.lightValue += lightValueLose;
        // The box isn't hidden here: a closing dialogue step should stay
        // readable until the player presses interact again.
        questlineCompletedEvent?.Invoke(this);
    }

    void OnLightValueChanged(float currentLightValue)
    {
        if (questLineStarted)
        {
            return;
        }

        if (currentLightValue >= lightValueToStartQuestLine)
        {
            state = QuestState.Started;
        }
    }

    void OnValidate()
    {
        state = _state;
    }
}
