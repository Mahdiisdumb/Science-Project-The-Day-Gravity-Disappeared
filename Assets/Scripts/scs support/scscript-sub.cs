using System;
using UnityEngine;

[Serializable]
public class SequenceStep
{
    public StepType type;

    [TextArea(6, 20)]
    public string ccscript;

    public CharacterRef actor;

    public float waitTime = 1f;

    [Header("AUTO")]
    public bool requirePrompt;

    [HideInInspector] public string soundId;
    [HideInInspector] public string effectId;
}

public enum StepType
{
    Dialogue,
    Pause,
    Sound,
    Effect
}