// https://stackoverflow.com/a/48817315/
// https://stackoverflow.com/a/40578161/

///*
#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CustomSceneLoader
{
    static readonly string desiredSceneName = "Outside";

    static CustomSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadCustomScene;
    }

    static void LoadCustomScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode && SceneManager.GetActiveScene().name != desiredSceneName) {
            SceneManager.LoadScene(desiredSceneName);

            // Clear console from errors thrown before the desired scene was loaded
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}

#endif
/**/
