using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource source;

    [System.Serializable]
    public class SoundEntry
    {
        public string id;
        public AudioClip clip;
    }

    public List<SoundEntry> sounds = new();

    Dictionary<string, AudioClip> map = new();

    AudioClip currentLoop;

    void Awake()
    {
        map.Clear();

        foreach (var s in sounds)
        {
            if (s == null || s.clip == null) continue;
            map[s.id.ToLower()] = s.clip;
        }
    }

    void ApplyModifiers(string id)
    {
        source.loop = false;
        source.pitch = 1f;

        if (id.Contains("_lowpitch"))
            source.pitch = 0.8f;

        if (id.Contains("_highpitch"))
            source.pitch = 1.2f;
    }

    string CleanId(string id)
    {
        return id
            .Replace("_lowpitch", "")
            .Replace("_highpitch", "")
            .Replace("_loop", "")
            .ToLower();
    }

    public IEnumerator PlayAndWait(string id)
    {
        if (string.IsNullOrEmpty(id))
            yield break;

        bool loop = id.Contains("_loop");

        string clean = CleanId(id);

        if (!map.TryGetValue(clean, out var clip))
        {
            Debug.LogWarning("Missing sound: " + id);
            yield break;
        }

        ApplyModifiers(id);

        if (loop)
        {
            // stop previous loop
            if (source.isPlaying)
                source.Stop();

            currentLoop = clip;
            source.clip = clip;
            source.loop = true;
            source.Play();

            yield break; // IMPORTANT: do NOT continue automatically
        }
        else
        {
            source.loop = false;
            source.clip = clip;
            source.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }

    public void StopLoop()
    {
        if (source.loop)
        {
            source.Stop();
            source.loop = false;
            currentLoop = null;
        }
    }
}