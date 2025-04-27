using System.Collections.Generic;
using System.Linq;
using log4net.Util;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class CraftManagerEditor : EditorWindow
{

    GameObject pattern;
    GameObject resultCraft;

    int currentCraftIdx = 0;

    [MenuItem("Tools/Craft Manager #m")]
    static void ShowWindow()
    {
        GetWindow<CraftManagerEditor>();
    }

    void AddNewCraft() {
        string path = EditorUtility.SaveFilePanel(
            "where ?", "Assets/Resources/Data/Crafts", "NewCraft", "assets"
        );
        if (path == "") return;

        path = path.Replace(Application.dataPath, "Assets");

        CraftDataObject newCraft = ScriptableObject.CreateInstance<CraftDataObject>();
        AssetDatabase.CreateAsset( newCraft, path );
        AssetDatabase.SaveAssets();
        
    }
    void RemCurrentCraft() {
    
    }

    private void OnGUI()
    {
        //Get the current craft data
        CraftDataObject currentCraftData = null;
        List<CraftDataObject> crafts = AssetDatabase.FindAssets("t:CraftDataObject")
            .Select(guid => (CraftDataObject) AssetDatabase.LoadAssetAtPath( 
                AssetDatabase.GUIDToAssetPath(guid)    
            , typeof(CraftDataObject))).ToList();

        //Top bar of the tool
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Popup(0, crafts.Select(o => o.name).ToArray());
        if(GUILayout.Button("+")) AddNewCraft();
        if(GUILayout.Button("-")) RemCurrentCraft();
        EditorGUILayout.EndHorizontal();


        //Display of the craft
        pattern = (GameObject) EditorGUILayout.ObjectField("craft pattern: ", pattern, typeof(GameObject), false);
        resultCraft = (GameObject) EditorGUILayout.ObjectField("craft result: ", resultCraft, typeof(GameObject), false);

    }


}
