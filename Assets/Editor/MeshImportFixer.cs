using UnityEngine;
using UnityEditor;

public class MeshImportFixer : EditorWindow
{
    [MenuItem("Tools/Fix Mesh Read/Write")]
    public static void FixMeshSettings()
    {
        string[] meshGuids = AssetDatabase.FindAssets("t:Mesh", new[] { "Assets" });
        foreach (string guid in meshGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                Debug.Log($"Fixed Read/Write setting for: {path}");
            }
        }
        Debug.Log("Completed fixing mesh Read/Write settings");
    }
}
