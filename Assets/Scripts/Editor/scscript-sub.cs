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

    public bool requirePrompt;

    public string soundId;
    public string effectId;
}

public enum StepType
{
    Dialogue,
    Pause,
    Sound,
    Effect
}

public class CharacterRef : MonoBehaviour
{
    public string id;
}