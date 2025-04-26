using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Printing;


public class RecipesManagerEditor : EditorWindow
{
    RecipeObject currentRecipe;
    List<RecipeObject> allRecipes = new List<RecipeObject>();

    int currentRecipeIndex;

    [MenuItem("Tools/Recipes Manager #r")]
    public static void ShowWindow()
    {
        var a = GetWindow<RecipesManagerEditor>();
        a.Show();
        a.Refresh();
    }

    void CreateRecipeObject()
    {
        // open file-expoler editor, for the new object
        string savePath = EditorUtility.SaveFilePanel("Where to save?", "Assets/Resources/Data", "NewRecipe", "asset");
        if (savePath == "") return;

        // convert abs to rel path
        savePath = savePath.Replace(Application.dataPath, "Assets");
        Debug.Log(savePath);
        
        // generate default file
        RecipeObject recipe = ScriptableObject.CreateInstance<RecipeObject>();
        AssetDatabase.CreateAsset(recipe, savePath);

        // select the new recipe
        Refresh();
        int idx = allRecipes.FindIndex(r => r == recipe);
        Debug.Log(idx);
        if (idx == -1) return;

    }

    void Refresh()
    {
        allRecipes = AssetDatabase.FindAssets("t:RecipeObject")
            .Select(guid => AssetDatabase.LoadAssetAtPath<RecipeObject>( AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();
    }

    public void OnGUI()
    {
        // refresh
        GUILayout.BeginHorizontal();
        currentRecipeIndex = EditorGUILayout.Popup(currentRecipeIndex, allRecipes.Select(x=>x.name).ToArray());
        if (GUILayout.Button("Refresh")) Refresh();
        GUILayout.EndHorizontal();
        // new object
        if (GUILayout.Button("New Recipe")) CreateRecipeObject();

        // get the recipe
        if (allRecipes.Count == 0) return;
        currentRecipe = allRecipes[currentRecipeIndex];

        // fields
        if (currentRecipe == null) return;
        currentRecipe.craftResult = (CardBase) EditorGUILayout.ObjectField("crafted card: ", currentRecipe.craftResult, typeof(CardBase),false);



    }
    
}
