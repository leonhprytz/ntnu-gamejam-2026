using System;
using UnityEngine;

public enum QuestStepKind
{
    Dialogue,
    Logic,
}

// One alternative offered after a dialogue step. Picking it sends the questline
// to targetStep instead of the step that simply comes next.
[Serializable]
public class QuestBranch
{
    [Tooltip("Shown to the player as a numbered alternative.")]
    public string option;

    [Tooltip("Index into the questline's steps array to continue from.")]
    public int targetStep;
}

// One beat of a QuestLine: either lines typed straight into the inspector, or a
// checkpoint script that runs some logic. A step is one or the other, never both.
[Serializable]
public class QuestStep
{
    public QuestStepKind kind;

    [Tooltip("One interact press per line.")]
    [TextArea] public string[] lines;

    [Tooltip("Optional: leave empty to just continue with the next step.")]
    public QuestBranch[] branches;

    [Tooltip("Runs on interact and completes the step itself.")]
    public InteractionCheckpoint logic;

    public bool IsDialogue => kind == QuestStepKind.Dialogue;

    public bool HasBranches => IsDialogue && branches != null && branches.Length > 0;
}
