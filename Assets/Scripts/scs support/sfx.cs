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

    void Awake()
    {
        map.Clear();

        foreach (var s in sounds)
        {
            if (s == null || s.clip == null) continue;
            map[s.id.ToLower()] = s.clip;
        }
    }

    public void Play(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        id = id.ToLower();

        if (!map.TryGetValue(id, out var clip))
        {
            Debug.LogWarning("Missing sound: " + id);
            return;
        }

        source.PlayOneShot(clip);
    }
}