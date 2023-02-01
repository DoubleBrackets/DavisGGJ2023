#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

public struct SceneData
{
    public SceneAsset asset;
    public string projectPath;

    public SceneData(SceneAsset asset, string projectPath)
    {
        this.asset = asset;
        this.projectPath = projectPath;
    }
}

public class SceneGrabber : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTree;

    [MenuItem("MushiTools/SceneGrabber")]
    public static void ShowExample()
    {
        SceneGrabber wnd = GetWindow<SceneGrabber>("Scene Grabber");
    }

    private VisualElement sceneList;
    private VisualElement searchParameters;

    private string folderPath = "Assets/Scenes";

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Grab important visual elements
        sceneList = root.Query<ScrollView>("SceneList");
        searchParameters = root.Query<VisualElement>("SearchParameters");

        // Refresh Scene Button
        Button refreshSceneButton = root.Query<VisualElement>("Header").First().Query<Button>("RefreshSceneButton");
        refreshSceneButton.clicked += GenerateSceneListFromAssets;

        TextField searchField = searchParameters.Query<TextField>("SearchPath");
        searchField.SetValueWithoutNotify(folderPath);
        searchField.RegisterValueChangedCallback((changeEvent) =>
        {
            folderPath = changeEvent.newValue;
            GenerateSceneListFromAssets();
        });

        // Initial setup
        GenerateSceneListFromAssets();
    }

    private void GenerateSceneListFromAssets()
    {
        var scenes = FindScenesData();
        CreateSceneList(scenes);
    }

    private void CreateSceneList(List<SceneData> scenes)
    {
        sceneList.Clear();
        foreach (var sceneData in scenes)
        {
            Button sceneButton = new Button(() => LoadScene(sceneData.projectPath)) { name = $"{sceneData.asset.name}Button", text = sceneData.asset.name };
            sceneList.Add(sceneButton);
        }
    }

    private void LoadScene(string scenePath)
    {
        try
        {
            EditorSceneManager.OpenScene(scenePath);
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(ArgumentException))
            {
                Debug.LogWarning("Scene path not found, refreshing list");
                GenerateSceneListFromAssets();
            }
            else
            {
                Debug.LogError(e);
            }
        }
    }

    private List<SceneData> FindScenesData()
    {
        var sceneGuids = FindScenesGuid();
        var paths = sceneGuids.Select(AssetDatabase.GUIDToAssetPath);
        List<SceneData> scenes = paths.Select(
                path => new SceneData(AssetDatabase.LoadAssetAtPath<SceneAsset>(path), path))
            .ToList();
        return scenes;
    }

    private IEnumerable<string> FindScenesGuid()
    {
        if (folderPath == string.Empty)
        {
            return AssetDatabase.FindAssets("t:SceneAsset");
        }
        else if (AssetDatabase.IsValidFolder(folderPath))
        {
            return AssetDatabase.FindAssets("t:SceneAsset", new string[] { folderPath });
        }

        return Enumerable.Empty<string>();
    }
}