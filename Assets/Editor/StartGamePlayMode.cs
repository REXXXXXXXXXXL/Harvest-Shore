using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartGamePlayMode
{
    private const string StartScenePath = "Assets/Scenes/StartGame.unity";

    static StartGamePlayMode()
    {
        var startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(StartScenePath);

        if (startScene != null)
            EditorSceneManager.playModeStartScene = startScene;
    }
}
