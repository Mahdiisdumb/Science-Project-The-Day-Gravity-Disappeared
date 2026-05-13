using UnityEditor;
using UnityEngine;

public class SpriteAutoSlicer : EditorWindow
{
    [MenuItem("Tools/Auto Slice 4x4 Sprite Sheet")]
    public static void SliceSelected()
    {
        Texture2D texture = Selection.activeObject as Texture2D;

        if (texture == null)
        {
            Debug.LogError("Select a texture first.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Not a valid texture.");
            return;
        }

        // Force correct settings
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.isReadable = false;
        importer.mipmapEnabled = false;

        int columns = 4;
        int rows = 4;

        int width = texture.width;
        int height = texture.height;

        int cellWidth = width / columns;
        int cellHeight = height / rows;

        SpriteMetaData[] sprites = new SpriteMetaData[columns * rows];

        int index = 0;

        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                SpriteMetaData smd = new SpriteMetaData
                {
                    name = $"mahdi_{index}",
                    rect = new Rect(
                        x * cellWidth,
                        y * cellHeight,
                        cellWidth,
                        cellHeight
                    ),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f)
                };

                sprites[index] = smd;
                index++;
            }
        }

        importer.spritesheet = sprites;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log("4x4 sprite sheet sliced successfully.");
    }
}