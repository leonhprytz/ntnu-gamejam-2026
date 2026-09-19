using System;
using UnityEngine;

public enum QuestStepKind
{
    Dialogue,
    Logic,
}

// One beat of a QuestLine: either lines typed straight into the inspector, or a
// checkpoint script that runs some logic. A step is one or the other, never both.
[Serializable]
public class QuestStep
{
    public QuestStepKind kind;

    [Tooltip("One interact press per line.")]
    [TextArea] public string[] lines;

    [Tooltip("Runs on interact and completes the step itself.")]
    public InteractionCheckpoint logic;

    public bool IsDialogue => kind == QuestStepKind.Dialogue;
}
