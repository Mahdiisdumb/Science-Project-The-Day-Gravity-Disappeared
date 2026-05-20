using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("TEXT")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("PROMPT")]
    public GameObject promptIcon;

    [Header("FONTS")]
    public List<UIFontEntry> fonts = new();

    Dictionary<string, TMP_FontAsset> fontMap =
        new Dictionary<string, TMP_FontAsset>();

    TMP_FontAsset currentFont;

    void Awake()
    {
        if (speakerText == null)
            Debug.LogError("[UI] speakerText NOT assigned");

        if (dialogueText == null)
            Debug.LogError("[UI] dialogueText NOT assigned");

        foreach (UIFontEntry entry in fonts)
        {
            if (entry == null || entry.font == null)
                continue;

            string key = entry.id.ToLower().Trim();

            if (!fontMap.ContainsKey(key))
                fontMap.Add(key, entry.font);
        }

        if (fonts.Count > 0 && fonts[0].font != null)
            currentFont = fonts[0].font;

        ApplyCurrentFont();
    }

    public void ShowDialogue(string speaker, string text)
    {
        if (speakerText == null || dialogueText == null)
            return;

        speakerText.text = speaker;

        dialogueText.text = text;

        ApplyCurrentFont();
    }

    void ApplyCurrentFont()
    {
        if (currentFont == null)
            return;

        dialogueText.font = currentFont;
    }

    public void SetFont(string fontId)
    {
        string key = fontId.ToLower().Trim();

        if (fontMap.TryGetValue(key, out TMP_FontAsset found))
        {
            currentFont = found;

            ApplyCurrentFont();
        }
        else
        {
            Debug.LogWarning($"[UI] Font '{fontId}' not found");
        }
    }

    public void ShowPrompt()
    {
        if (promptIcon != null)
            promptIcon.SetActive(true);
    }
}
[System.Serializable]
public class UIFontEntry
{
    public string id;
    public TMP_FontAsset font;
}