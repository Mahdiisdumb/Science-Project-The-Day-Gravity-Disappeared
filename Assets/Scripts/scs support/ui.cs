using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public GameObject promptIcon;

    void Awake()
    {
        if (speakerText == null)
            Debug.LogError("[UI] speakerText NOT assigned");

        if (dialogueText == null)
            Debug.LogError("[UI] dialogueText NOT assigned");
    }

    public void ShowDialogue(string speaker, string text)
    {
        if (speakerText == null || dialogueText == null)
        {
            Debug.LogError("[UI] Missing TMP references");
            return;
        }

        speakerText.text = speaker;
        speakerText.color = Color.black;   // 🔥 FORCE NAME COLOR BLACK

        dialogueText.text = text;
    }

    public void ShowPrompt()
    {
        if (promptIcon != null)
            promptIcon.SetActive(true);
    }
}