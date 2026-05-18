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
            if (c == null) continue;
            map[c.id.ToLower()] = c;
        }
    }

    public void Play(string effect, string targetId)
    {
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

        StartCoroutine(PlayAnim(c, effect));
    }

    System.Collections.IEnumerator PlayAnim(CharacterRef c, string effect)
    {
        var anim = c.animator;

        // enable only when needed
        anim.enabled = true;

        anim.Play(effect);

        // wait until animation starts playing
        yield return null;

        // wait for it to finish
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // disable again (freeze character)
        anim.enabled = false;
    }
}