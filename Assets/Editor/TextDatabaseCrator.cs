using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextDatabaseCrator : EditorWindow
{
    const string kXUIStyleDirectoryPath = "Assets/Resources/TextDatabase/";

    [MenuItem("Tools/CreateTextDatabase")]
    static void CreateStyleSheet()
    {
        CreateStyleDirectoryIfNotExist();
        string styleAssetPath = Path.Combine(kXUIStyleDirectoryPath, "TextDatabase.asset");
        styleAssetPath = AssetDatabase.GenerateUniqueAssetPath(styleAssetPath);
        TextDatabase asset = ScriptableObject.CreateInstance<TextDatabase>();
        AssetDatabase.CreateAsset(asset, styleAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    static void CreateStyleDirectoryIfNotExist()
    {
        string rootPath = Application.dataPath;

        string resourceDirPath = Path.Combine(rootPath, "Resources");
        if (!Directory.Exists(resourceDirPath))
        {
            Directory.CreateDirectory(resourceDirPath);
            AssetDatabase.Refresh();
        }

        string styleDirPath = Path.Combine(resourceDirPath, "TextDatabase");
        if (!Directory.Exists(styleDirPath))
        {
            Directory.CreateDirectory(styleDirPath);
            AssetDatabase.Refresh();
        }
    }

}
