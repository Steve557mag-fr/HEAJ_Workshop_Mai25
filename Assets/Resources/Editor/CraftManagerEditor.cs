using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CraftManagerEditor : EditorWindow
{

    GameObject pattern;
    GameObject resultCraft;
    List<CraftDataObject> crafts;

    int currentCraftIdx = 0;

    [MenuItem("Tools/Craft Manager #m")]
    static void ShowWindow()
    {
        GetWindow<CraftManagerEditor>();
    }

    void AddNewCraft() {
        string path = EditorUtility.SaveFilePanel(
            "where ?", "Assets/Resources/Data/Crafts", "NewCraft", "asset"
        );
        if (path == "") return;

        path = path.Replace(Application.dataPath, "Assets");

        CraftDataObject newCraft = ScriptableObject.CreateInstance<CraftDataObject>();
        AssetDatabase.CreateAsset( newCraft, path );
        AssetDatabase.SaveAssets();

    }
    void RemCurrentCraft() {
        CraftDataObject currentCraftData = crafts[currentCraftIdx];
        if (currentCraftData == null) return;

        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentCraftData));
        AssetDatabase.SaveAssets();

    }

    private void OnGUI()
    {
        //Get the current craft data
        CraftDataObject currentCraftData = null;
        crafts = AssetDatabase.FindAssets("t:CraftDataObject")
            .Select(guid => (CraftDataObject) AssetDatabase.LoadAssetAtPath( 
                AssetDatabase.GUIDToAssetPath(guid)    
            , typeof(CraftDataObject))).ToList();

        //Top bar of the tool
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        currentCraftIdx = EditorGUILayout.Popup(currentCraftIdx, crafts.Select(o => o.name).ToArray());
        if(GUILayout.Button("+")) AddNewCraft();
        if(GUILayout.Button("-")) RemCurrentCraft();
        EditorGUILayout.EndHorizontal();
        EditorUtils.GuiLine(padding: 15);

        if (currentCraftIdx >= crafts.Count || currentCraftIdx < 0) return;
        currentCraftData = crafts[currentCraftIdx];

        //Display of the craft
        currentCraftData.pattern = 
            (GameObject) EditorGUILayout.ObjectField("craft pattern: ", currentCraftData.pattern, typeof(GameObject), false);

        currentCraftData.craftResult = 
            (GameItemObject) EditorGUILayout.ObjectField("craft result: ", currentCraftData.craftResult, typeof(GameItemObject), false);

    }


}
