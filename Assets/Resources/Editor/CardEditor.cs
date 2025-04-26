using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.Overlays;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardEditor : EditorWindow
{
    //Card Stats
    private string currentCardName;
    private Sprite currentCardImage;

    //Delete Dropdown menu 
    private System.Type[] loadedCardTypes;
    private List<CardBase> loadedCards = new List<CardBase>();
    private string[] cardManagerOption = {"", "Create", "Modify", "Delete"};
    private string[] loadedCardClasses;
    private string[] loadedCardNames;
    private int classDPSelectedIndex = 0;
    private int deleteDPSelectedIndex = 0;
    private int modifyDPSelectedIndex = 0;

    [MenuItem("Tools/Card Manager #m")]
    public static void ShowWindow()
    {
        GetWindow<CardEditor>("Card Generator");
    }

    private void OnEnable()
    {
        Refresh();
        LoadCardClass();
    }

    void Refresh()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardBase");
        loadedCards = guids.Select(guid => AssetDatabase.LoadAssetAtPath<CardBase>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        loadedCardNames = loadedCards.Select(card => card.name).ToArray();

    }

    void LoadCardClass()
    {
        //load class
        loadedCardTypes = ReflectionUtils.GetClassesOf<CardBase>();
        loadedCardClasses = loadedCardTypes.Select(c => c.Name).ToArray();

    }

    void GenerateCard(string name, Sprite image, int classIndex)
    {
        string path = EditorUtility.SaveFilePanel("Where ?", "Assets/Resources/Data/Cards", "NewCard", "asset");
        if (path == "") return;

        path = path.Replace(Application.dataPath, "Assets");

        CardBase card = ScriptableObject.CreateInstance<CardBase>();
        AssetDatabase.CreateAsset(card, path);
        AssetDatabase.SaveAssets();
        Refresh();

        int cardIndex = loadedCards.FindIndex(c => c == card);
        deleteDPSelectedIndex = cardIndex;
    }
    CardBase LoadModifyCard(int index)
    {
        if (index >= 0 && index < loadedCards.Count) return loadedCards[index];
        else return null;
    }


    void OnGUI ()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();   
        if (loadedCards.Count > 0)
        {
            deleteDPSelectedIndex = EditorGUILayout.Popup("Choose Your Card", deleteDPSelectedIndex, loadedCardNames);
        }
        else GUILayout.Label("No cards found.", EditorStyles.boldLabel);
        //Delete button that will also save the asset change in memory and reload the dropdown
        if (GUILayout.Button("Delete Card"))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(loadedCards[deleteDPSelectedIndex]));
            Debug.Log("Card Deleted");
            AssetDatabase.SaveAssets();
            Debug.Log("Saved");
            deleteDPSelectedIndex = 0; 
            Refresh();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (loadedCardClasses.Length > 0)
        {
            GUILayout.Label("Choose A Card Type");
            classDPSelectedIndex = EditorGUILayout.Popup("", classDPSelectedIndex, loadedCardClasses);
        }
        else GUILayout.Label("No cards found.", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate New Card", GUILayout.Width(150)))
        {
            GenerateCard(currentCardName, currentCardImage, classDPSelectedIndex);

        }
        GUILayout.EndHorizontal();

        CardBase loadedCard = LoadModifyCard(deleteDPSelectedIndex);
        if (loadedCard == null) return;

        GUILayout.Space(10);

        EditorUtils.GuiLine();

        GUILayout.Space(10);
        loadedCard.cardTitle = EditorGUILayout.TextField(" Name", loadedCard.cardTitle);
        loadedCard.image = EditorGUILayout.ObjectField("Card Image", loadedCard.image, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
    }


}
