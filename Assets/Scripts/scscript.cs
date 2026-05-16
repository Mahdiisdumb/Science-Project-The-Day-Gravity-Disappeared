using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class ScScriptRuntime : MonoBehaviour
{
    [Header("SCRIPT")]
    [TextArea(10, 60)]
    public string script;

    [Header("REFERENCES")]
    public UIController ui;
    public AudioSource typeAudio;
    public AudioClip typeClip;

    [Header("CHARACTERS / WORLD")]
    public List<CharacterRef> characters;
    public List<WorldRef> worldObjects;

    [Header("TYPEWRITER SETTINGS")]
    public float typeSpeed = 0.02f;
    public bool soundPerChar = true;

    [Header("EVENTS")]
    public UnityEvent onPrompt;
    public UnityEvent<string> onSound;
    public UnityEvent<string> onEffect;

    Dictionary<string, CharacterRef> charMap = new();
    Dictionary<string, WorldRef> worldMap = new();

    bool waitingForPrompt;
    bool skipTyping;

    void Awake()
    {
        BuildMaps();
    }

    void Start()
    {
        StartCoroutine(RunScript());
    }

    void BuildMaps()
    {
        foreach (var c in characters)
            if (c != null)
                charMap[Norm(c.id)] = c;

        foreach (var w in worldObjects)
            if (w != null)
                worldMap[Norm(w.id)] = w;
    }

    string Norm(string s)
    {
        return s.ToLower().Replace(" ", "").Replace("_", "");
    }

    IEnumerator RunScript()
    {
        string src = script.Replace("\r\n", "\n");

        var lines = src.Split('\n');

        foreach (var raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            yield return ProcessLine(line);
        }
    }

    IEnumerator ProcessLine(string line)
    {
        string speaker = "";
        string rest = line;

        int colon = line.IndexOf(':');
        if (colon > 0)
        {
            speaker = line.Substring(0, colon).Trim();
            rest = line.Substring(colon + 1).Trim();
        }

        yield return ProcessSegments(speaker, rest);
    }

    IEnumerator ProcessSegments(string speaker, string text)
    {
        var pattern = new Regex(
            @"\*(?<action>.*?)\*|" +
            "\"(?<dialogue>.*?)\"|" +
            @"\{(?<func>[^}]*)\}",
            RegexOptions.Singleline
        );

        foreach (Match m in pattern.Matches(text))
        {
            if (m.Groups["action"].Success)
            {
                yield return HandleAction(speaker, m.Groups["action"].Value);
            }
            else if (m.Groups["dialogue"].Success)
            {
                yield return HandleDialogue(speaker, m.Groups["dialogue"].Value);
            }
            else if (m.Groups["func"].Success)
            {
                yield return HandleFunction(m.Groups["func"].Value);
            }
        }
    }

    IEnumerator HandleDialogue(string speaker, string raw)
    {
        yield return FireFuncs(raw);

        string clean = Regex.Replace(raw, @"\{[^}]*\}", "");
        clean = clean.Replace("/n", "\n").Trim();

        clean = clean.Replace("@", "λ");

        yield return TypeText(speaker, clean);
    }

    IEnumerator TypeText(string speaker, string text)
    {
        string current = "";

        ui.ShowDialogue(speaker, "");

        foreach (char c in text)
        {
            if (waitingForPrompt) yield break;

            current += c;

            ui.ShowDialogue(speaker, current);

            if (soundPerChar && c != ' ')
                typeAudio?.PlayOneShot(typeClip);

            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator HandleAction(string actor, string action)
    {
        yield return FireFuncs(action);

        string clean = Regex.Replace(action, @"\{[^}]*\}", "").ToLower();

        if (clean.Contains("fade"))
        {
            onEffect?.Invoke(clean.Contains("out") ? "fade_out" : "fade_in");
        }

        var turn = Regex.Match(clean, @"(\w+)?\s*turns?\s+to\s+(\w+)");
        if (turn.Success)
        {
            string a = Norm(turn.Groups[1].Value == "" ? actor : turn.Groups[1].Value);
            string b = Norm(turn.Groups[2].Value);

            yield return TurnTo(a, b);
        }

        var move = Regex.Match(clean, @"(\w+)?\s*(goes|walks|runs|dashes)?\s*(to|over to)?\s*(\w+)");
        if (move.Success && move.Groups[4].Value != "")
        {
            string a = Norm(move.Groups[1].Value == "" ? actor : move.Groups[1].Value);
            string b = Norm(move.Groups[4].Value);

            yield return Move(a, b);
        }
    }

    IEnumerator HandleFunction(string f)
    {
        var m = Regex.Match(f, @"(\w+)\(?([^)]*)\)?");

        string name = m.Groups[1].Value.ToLower();
        string arg = m.Groups[2].Value;

        switch (name)
        {
            case "pause":
                if (int.TryParse(arg, out int ms))
                    yield return new WaitForSeconds(ms / 1000f);
                break;

            case "sound":
                onSound?.Invoke(arg);
                break;

            case "effect":
                onEffect?.Invoke(arg);
                break;

            case "promt":
            case "prompt":
                onPrompt?.Invoke();
                waitingForPrompt = true;
                while (waitingForPrompt) yield return null;
                break;
        }
    }

    IEnumerator FireFuncs(string text)
    {
        var matches = Regex.Matches(text, @"\{([^}]*)\}");

        foreach (Match m in matches)
            yield return HandleFunction(m.Groups[1].Value);
    }

    public void Resume()
    {
        waitingForPrompt = false;
    }

    IEnumerator TurnTo(string a, string b)
    {
        if (!charMap.ContainsKey(a) || !charMap.ContainsKey(b))
            yield break;

        var ca = charMap[a].transform;
        var cb = charMap[b].transform;

        Vector3 dir = (cb.position - ca.position).normalized;
        ca.forward = dir;

        yield return null;
    }

    IEnumerator Move(string a, string b)
    {
        if (!charMap.ContainsKey(a)) yield break;

        Transform t = charMap[a].transform;

        Vector3 target;

        if (charMap.ContainsKey(b))
            target = charMap[b].transform.position;
        else if (worldMap.ContainsKey(b))
            target = worldMap[b].transform.position;
        else yield break;

        while (Vector3.Distance(t.position, target) > 0.1f)
        {
            t.position = Vector3.MoveTowards(t.position, target, Time.deltaTime * 2f);
            yield return null;
        }
    }
}