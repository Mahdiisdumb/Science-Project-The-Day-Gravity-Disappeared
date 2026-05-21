using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public List<CharacterRef> characters;

    Dictionary<string, CharacterRef> map = new();

    void Awake()
    {
        map.Clear();

        foreach (var c in characters)
        {
            if (c == null)
                continue;

            map[c.id.ToLower()] = c;
        }
    }

    public void Play(string rawEffect, string targetId)
    {
        if (string.IsNullOrWhiteSpace(targetId))
        {
            Debug.LogWarning("No target specified.");
            return;
        }

        if (!map.TryGetValue(targetId.ToLower(), out var c))
        {
            Debug.LogWarning("Missing character: " + targetId);
            return;
        }

        if (c.animator == null)
        {
            Debug.LogWarning("No animator on: " + targetId);
            return;
        }

        ParseEffect(rawEffect, out string animName, out HashSet<string> tags);

        StartCoroutine(PlayAnim(c, animName, tags));
    }

    void ParseEffect(string raw, out string animName, out HashSet<string> tags)
    {
        tags = new HashSet<string>();

        string[] split = raw.Split('_');

        animName = split[0];

        for (int i = 1; i < split.Length; i++)
            tags.Add(split[i].ToLower());
    }

    IEnumerator PlayAnim(CharacterRef c, string animName, HashSet<string> tags)
    {
        Animator anim = c.animator;

        anim.enabled = true;

        anim.Play(animName);

        yield return null;

        bool loop = tags.Contains("loop");

        if (!loop)
        {
            yield return new WaitForSeconds(
                anim.GetCurrentAnimatorStateInfo(0).length
            );

            anim.enabled = false;
        }
    }
}