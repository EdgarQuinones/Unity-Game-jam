using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only: place the approved visual prefab without overwriting an open, dirty scene.
[InitializeOnLoad]
public static class TempleDecorationInstaller
{
    private const string ScenePath = "Assets/_Project/Scenes/InsideTemple.unity";
    private const string PrefabPath = "Assets/_Project/Prefabs/TempleDecorations.prefab";
    private const string SessionKey = "TempleDecorations.InitialPlacement.v1";

    static TempleDecorationInstaller()
    {
        EditorApplication.delayCall += TryInitialPlacement;
    }

    private static void TryInitialPlacement()
    {
        if (SessionState.GetBool(SessionKey, false) || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += TryInitialPlacement;
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath || !scene.isLoaded)
            return;

        if (AddToScene(scene))
            SessionState.SetBool(SessionKey, true);
    }

    [MenuItem("Tools/Temple/Add Approved Decorations to InsideTemple")]
    private static void AddFromMenu()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlayingOrWillChangePlaymode || scene.path != ScenePath)
        {
            Debug.LogWarning("Open InsideTemple outside Play Mode before adding its decorations.");
            return;
        }

        if (AddToScene(scene))
            SessionState.SetBool(SessionKey, true);
    }

    private static bool AddToScene(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "TempleDecorations")
                return true;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("Temple decoration prefab is not imported yet. Use Tools/Temple/Add Approved Decorations to InsideTemple after import.");
            return false;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
        if (instance == null)
            return false;

        Undo.RegisterCreatedObjectUndo(instance, "Add temple decorations");
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("TempleDecorations added to the open InsideTemple scene with Undo support. Existing scene work was preserved; save the scene to include the decorations in your build.", instance);
        return true;
    }
}