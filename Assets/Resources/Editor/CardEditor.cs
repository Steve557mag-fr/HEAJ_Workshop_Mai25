using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class CardEditor : EditorWindow
{
    //Card Stats
    private string currentCardName;
    private Sprite currentCardImage;

    //Delete Dropdown menu 
    private System.Type[] loadedCardTypes;
    private CardBase[] loadedCards;
    private string[] cardManagerOption = {"", "Create", "Modify", "Delete"};
    private string[] loadedCardClasses;
    private string[] loadedCardNames;
    private int classDPSelectedIndex = 0;
    private int deleteDPSelectedIndex = 0;
    private int modifyDPSelectedIndex = 0;

    Option option;

    [MenuItem("Tools/Card Manager #m")]
    public static void ShowWindow()
    {
        GetWindow<CardEditor>("Card Generator");
    }

    private void OnEnable()
    {
        LoadCards();
        LoadCardClass();
        option = Option.CREATE;
    }

    void LoadCards()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardBase");
        loadedCards = guids.Select(guid => AssetDatabase.LoadAssetAtPath<CardBase>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
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
        CardBase card = ScriptableObject.CreateInstance<CardBase>();
        AssetDatabase.CreateAsset(card, $"Assets/Resources/Data/Cards/{name}.asset");
        AssetDatabase.SaveAssets();
        LoadCards();
    }
    CardBase LoadModifyCard(int index)
    {
        return AssetDatabase.LoadAssetAtPath<CardBase>(loadedCardNames[index]);
    }

    void ApplyChanges(string name, Sprite image, CardBase card)
    {
        Debug.Log("Changes (Allegedly) Applied haha xd");
    }

    void DrawCreate()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Generate your dream card!", EditorStyles.boldLabel);

        //reloads cards if a new card was created without the generation window
        if (GUILayout.Button("Reload Cards", GUILayout.Width(100))) LoadCards();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.Label("Card Parameters", EditorStyles.boldLabel);
        GUILayout.Space(5);

        currentCardName = EditorGUILayout.TextField("Name", currentCardName);

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image");

        currentCardImage = EditorGUILayout.ObjectField(currentCardImage, typeof(Sprite), false) as Sprite;

        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (loadedCardClasses.Length > 0)
        {
            GUILayout.Label("Chose A Class");
            classDPSelectedIndex = EditorGUILayout.Popup("", classDPSelectedIndex, loadedCardClasses);
        }
        else GUILayout.Label("No cards found.", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Card", GUILayout.Width(150)))
        {
            GenerateCard(currentCardName, currentCardImage, classDPSelectedIndex);
            Debug.Log("Card Generated Successfully");
        }
        GUILayout.EndHorizontal();
    }

    void DrawDelete()
    {
        //introducing delete
        GUILayout.Space(15);
        GUILayout.Label("Delete A Card", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("If you just created a card and it doesn't show,\nPress 'Reload Cards'", MessageType.Info);
        GUILayout.Space(5);

        //if cards are found in the project
        if (loadedCards.Length > 0)
        {
            //create the dropdown 
            deleteDPSelectedIndex = EditorGUILayout.Popup("Choose a card :", deleteDPSelectedIndex, loadedCardNames);

            GUILayout.Space(5);

            //Delete button that will also save the asset change in memory and reload the dropdown
            if (GUILayout.Button("Delete Card"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(loadedCards[deleteDPSelectedIndex]));
                Debug.Log("Card Deleted");
                AssetDatabase.SaveAssets();
                Debug.Log("Saved");
                LoadCards();
            }
        }
        else GUILayout.Label("No cards found.", EditorStyles.boldLabel);
    }
    void DrawModify()
    {
        bool drawModifyIsActive = false;
        if (!drawModifyIsActive) drawModifyIsActive = true;
        GUILayout.Space(10);
        GUILayout.Label("Choose a Card to Modify");
        GUILayout.Space(5);

        modifyDPSelectedIndex = EditorGUILayout.Popup("", modifyDPSelectedIndex, loadedCardNames);
        CardBase loadedCard = LoadModifyCard(modifyDPSelectedIndex);

        GUILayout.Space(10);
        currentCardName = EditorGUILayout.TextField("Name", currentCardName);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        currentCardImage = EditorGUILayout.ObjectField(currentCardImage, typeof(Sprite), false) as Sprite;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Apply Changes")) ApplyChanges(currentCardName, currentCardImage, loadedCard);
    }

    void OnGUI ()
    {
        option = (Option)EditorGUILayout.Popup("", (int)option, cardManagerOption);

        switch(option)
        {
            case Option.CREATE:
                DrawCreate();
                break;
            case Option.MODIFY:
                DrawModify();
                break;
            case Option.DELETE:
                DrawDelete();
                break;

        }

    }

    enum Option
    {
        NONE,
        CREATE,
        MODIFY,
        DELETE
    }

}
