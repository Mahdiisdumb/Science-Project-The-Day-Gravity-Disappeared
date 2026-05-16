using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScScript : MonoBehaviour
{
    [Header("SEQUENCE")]
    public List<SequenceStep> steps = new();

    [Header("REFERENCES")]
    public UIController ui;
    public AudioSource typeAudio;
    public AudioClip typeClip;
    public EffectController effectController;
    public SoundController soundController;

    [Header("TYPEWRITER")]
    public float typeSpeed = 0.02f;
    public bool soundPerChar = true;

    bool waitingForPrompt;

    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        foreach (SequenceStep step in steps)
            yield return Execute(step);
    }

    IEnumerator Execute(SequenceStep step)
    {
        switch (step.type)
        {
            case StepType.Dialogue:
                yield return TypeDialogue(step);
                break;

            case StepType.Pause:
                yield return new WaitForSeconds(step.waitTime);
                break;

            case StepType.Sound:
                if (!string.IsNullOrWhiteSpace(step.soundId))
                    yield return soundController.PlayAndWait(step.soundId);
                break;

            case StepType.Effect:
                if (!string.IsNullOrWhiteSpace(step.effectId))
                    effectController?.Play(step.effectId, "");
                break;
        }

        if (step.requirePrompt)
        {
            waitingForPrompt = true;

            while (waitingForPrompt)
                yield return null;
        }
    }

    IEnumerator TypeDialogue(SequenceStep step)
    {
        if (ui == null)
            yield break;

        string speaker = step.actor != null
            ? step.actor.id
            : "";

        string text = step.ccscript.Replace("/n", "\n");

        ui.ShowDialogue(speaker, "");

        string current = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                int end = text.IndexOf('}', i);

                if (end > i)
                {
                    string cmd = text.Substring(i + 1, end - i - 1);

                    yield return HandleCommand(cmd);

                    i = end;
                    continue;
                }
            }

            current += text[i];

            ui.ShowDialogue(speaker, current);

            if (soundPerChar && text[i] != ' ')
                typeAudio?.PlayOneShot(typeClip);

            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator HandleCommand(string cmd)
    {
        Match m = Regex.Match(cmd, @"(\w+)\((.*?)\)");

        if (!m.Success)
        {
            string lower = cmd.ToLower().Trim();

            if (lower == "promt" || lower == "prompt")
            {
                waitingForPrompt = true;

                while (waitingForPrompt)
                    yield return null;
            }

            yield break;
        }

        string name = m.Groups[1].Value.ToLower();
        string arg = m.Groups[2].Value;

        switch (name)
        {
            case "pause":
                {
                    if (int.TryParse(arg, out int ms))
                        yield return new WaitForSeconds(ms / 1000f);

                    break;
                }

            case "sound":
                {
                    yield return soundController.PlayAndWait(arg);
                    break;
                }

            case "effect":
                {
                    string[] parts = arg.Split(',');

                    string fx = parts[0].Trim();

                    string target = parts.Length > 1
                        ? parts[1].Trim()
                        : "";

                    effectController?.Play(fx, target);

                    break;
                }

            case "prompt":
                {
                    waitingForPrompt = true;

                    while (waitingForPrompt)
                        yield return null;

                    break;
                }

            case "scene":
                {
                    if (int.TryParse(arg, out int sceneIndex))
                        SceneManager.LoadScene(sceneIndex);

                    break;
                }
        }
    }

    public void Continue()
    {
        waitingForPrompt = false;
    }
}