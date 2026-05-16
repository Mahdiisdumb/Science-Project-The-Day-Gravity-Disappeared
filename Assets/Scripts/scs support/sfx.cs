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
        foreach (var s in sounds)
            map[s.id.ToLower()] = s.clip;
    }

    public void Play(string id)
    {
        id = id.ToLower();

        if (!map.ContainsKey(id))
        {
            Debug.LogWarning("Missing sound: " + id);
            return;
        }

        source.PlayOneShot(map[id]);
    }
}