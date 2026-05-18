using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SequenceStep))]
public class SequenceStepDrawer : PropertyDrawer
{
    static bool showDocs = false;

    static bool coreFold = true;
    static bool typesFold = true;
    static bool inspectorFold = true;
    static bool commandsFold = true;
    static bool examplesFold = true;

    static Vector2 scroll;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = 220;

        if (showDocs)
            baseHeight += 260;

        return baseHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty type = property.FindPropertyRelative("type");
        SerializedProperty script = property.FindPropertyRelative("ccscript");
        SerializedProperty actor = property.FindPropertyRelative("actor");
        SerializedProperty waitTime = property.FindPropertyRelative("waitTime");
        SerializedProperty requirePrompt = property.FindPropertyRelative("requirePrompt");

        Rect r = position;
        r.height = 18;

        EditorGUI.PropertyField(r, type);

        r.y += 22;

        StepType t = (StepType)type.enumValueIndex;

        switch (t)
        {
            case StepType.Dialogue:
                EditorGUI.PropertyField(r, actor);
                r.y += 22;

                r.height = 90;
                EditorGUI.PropertyField(r, script);

                r.y += 95;
                r.height = 18;

                EditorGUI.PropertyField(r, requirePrompt);
                r.y += 24;
                break;

            case StepType.Pause:
                EditorGUI.PropertyField(r, waitTime);
                r.y += 22;
                break;
        }

        if (GUI.Button(new Rect(r.x, r.y, 160, 24), "Documentation"))
            showDocs = !showDocs;

        r.y += 28;

        if (showDocs)
        {
            Rect boxRect = new Rect(r.x, r.y, position.width, 260);
            GUI.Box(boxRect, "");

            Rect viewRect = new Rect(0, 0, position.width - 20, 800);

            scroll = GUI.BeginScrollView(
                new Rect(r.x + 5, r.y + 5, position.width - 10, 250),
                scroll,
                viewRect
            );

            float y = 0;

            y = DrawFoldout("CORE SYSTEMS", ref coreFold, y, pos =>
            {
                EditorGUI.LabelField(pos,
                    "- UIController → handles dialogue display\n" +
                    "- SoundController → plays SFX by ID\n" +
                    "- EffectController → runs animations/effects\n" +
                    "- CharacterRef → speaker identity system\n" +
                    "- ScScript → sequence runner");
            });

            y = DrawFoldout("STEP TYPES", ref typesFold, y, pos =>
            {
                EditorGUI.LabelField(pos,
                    "Dialogue → typewriter text\n" +
                    "Pause → wait seconds\n" +
                    "Sound → plays SFX\n" +
                    "Effect → triggers effect system");
            });

            y = DrawFoldout("INSPECTOR OPTIONS", ref inspectorFold, y, pos =>
            {
                EditorGUI.LabelField(pos,
                    "requirePrompt = true → pauses execution\n" +
                    "Use Continue() to resume flow");
            });

            y = DrawFoldout("COMMANDS", ref commandsFold, y, pos =>
            {
                EditorGUI.LabelField(pos,
                    "{pause(ms)}\n" +
                    "{sound(id)}\n" +
                    "{effect(name,target)}\n" +
                    "{scene(index)}\n" +
                    "{prompt} / {promt}\n" +
                    "/n = newline");
            });

            y = DrawFoldout("EXAMPLE", ref examplesFold, y, pos =>
            {
                EditorGUI.LabelField(pos,
                    "@Mahdi: What is this?\n" +
                    "{sound(explosion)}\n" +
                    "{effect(shake,mahdi)}\n" +
                    "{pause(300)}\n" +
                    "@Enemy: You’ll see.\n" +
                    "/n\n" +
                    "{prompt}");
            });

            GUI.EndScrollView();
        }
    }

    float DrawFoldout(string title, ref bool fold, float y, System.Action<Rect> draw)
    {
        Rect header = new Rect(0, y, 250, 18);
        fold = EditorGUI.Foldout(header, fold, title, true);

        y += 18;

        if (fold)
        {
            Rect content = new Rect(10, y, 250, 80);
            draw(content);
            y += 80;
        }

        return y;
    }
}