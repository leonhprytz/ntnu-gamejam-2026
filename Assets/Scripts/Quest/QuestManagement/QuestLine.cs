using System;
using System.Linq;
using UnityEngine;

// [CustomEditor(typeof(QuestLine))]
public class QuestLine : Interactable
{
    public string questName;
    public QuestStep[] steps;

    [Tooltip("Optional: found among this NPC's children when left empty.")]
    public DialogueInteraction dialogue;

    private int currentStepIndex = 0;

    [Header("light value settings")]
    public int lightValueToStartQuestLine;
    public int lightValueWin;
    public int lightValueLose;

    [Header("npc settings")]
    public bool spawnsNPC;
    public GameObject npcToSpawn;

    public event Action<QuestLine> questlineCompletedEvent;
    private bool questLineStarted = false;
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

        step.logic.Interact();
        CompleteCurrentStep();
        return true;
    }

    public void StartQuestLine()
    {
        Debug.Log("questline started");
        GoToStep(0);
        if (npcToSpawn != null)
        {
            npcToSpawn.SetActive(true);
        }
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
        currentStepIndex = Mathf.Clamp(stepIndex, 0, steps.Length);

        if (currentStepIndex < steps.Length)
        {
            if (this.steps[currentStepIndex].auto)
            {
                TryInteract();
            }
            return;
        }

        active = false; // completed deactivate
        // The box isn't hidden here: a closing dialogue step should stay
        // readable until the player presses interact again.
        questlineCompletedEvent?.Invoke(this);
    }

    void OnLightValueChanged(int currentLightValue)
    {
        if (questLineStarted)
        {
            return;
        }

        if (currentLightValue >= lightValueToStartQuestLine)
        {
            questLineStarted = true;
            StartQuestLine();
        }
    }
}
