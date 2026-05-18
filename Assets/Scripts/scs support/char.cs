using UnityEngine;

public class CharacterRef : MonoBehaviour
{
    public string id;
    public Animator animator;

    void Awake()
    {
        if (animator == null) return;

        // 🔥 stop any automatic animation playback
        animator.enabled = false;
    }
}