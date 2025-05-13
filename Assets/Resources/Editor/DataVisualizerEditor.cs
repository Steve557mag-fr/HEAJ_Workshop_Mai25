using UnityEditor;
using UnityEngine;


public class DataVisualizerEditor : EditorWindow
{

    string data;
    string filepath;

    [MenuItem("Tools/Data Visualizer #d")]
    public static void ShowWindow()
    {
        GetWindow<DataVisualizerEditor>();
    }

    private void OnGUI()
    {
        filepath = EditorGUILayout.TextField("save name: ", filepath);

        if(GUILayout.Button("Refresh")){
            data = DataSystem.GetRawData(filepath);
        }

        EditorGUILayout.TextArea(data);

    }

}
