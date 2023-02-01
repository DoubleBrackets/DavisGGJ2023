#region

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

public class RandomStuffWindow : EditorWindow
{
    [MenuItem("MushiTools/RandomStuffWindow")]
    public static void ShowWindow()
    {
        RandomStuffWindow wnd = GetWindow<RandomStuffWindow>("Random Stuff");
        SceneView.duringSceneGui += wnd.OnSceneGUI;
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Open Persistent Data Folder"))
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        // Show current selected UI object
        if (EventSystem.current != null)
            EditorGUILayout.ObjectField("Event System Selected GO", EventSystem.current.currentSelectedGameObject, typeof(GameObject), true);
    }

    public void OnSceneGUI(SceneView view)
    {
    }
}