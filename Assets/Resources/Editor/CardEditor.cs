using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine.Rendering;
using log4net.Core;
using System;

public class CardEditor : EditorWindow
{
    private string generatedCardName;
    private Sprite cardImage;

    private int generateSelectedIndex = 0;

    //Delete Dropdown menu 
    private int deleteSelectedIndex = 0;
    private CardBase[] cards;
    private string[] cardNames;

    [MenuItem("Window/Card Generator")]
    public static void ShowWindow()
    {
        GetWindow<CardEditor>("Card Generator");
    }

    private void OnEnable()
    {
        LoadCards();
    }

    void LoadCards()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardBase");
        cards = guids.Select(guid => AssetDatabase.LoadAssetAtPath<CardBase>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        cardNames = cards.Select(card => card.name).ToArray();
    }

    void GenerateCard()
    {
        // il faut ici choper le type de scriptableObject à partir du choix de l'utilisateur. 
        // soit je suis très con soit je vois pas comment faire parce que le choix est une variable en string
        // et on peut pas simplement faire MyScriptableObject asset = ...; comme tout le monde fait

    }

    void OnGUI ()
    {
        GUILayout.Label("Generate your dream card!", EditorStyles.boldLabel);
        
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.Label("Card Parameters", EditorStyles.boldLabel);

        generatedCardName = EditorGUILayout.TextField("Name", generatedCardName);
        GUILayout.Label("Image");
        cardImage = EditorGUILayout.ObjectField(cardImage, typeof(Sprite), false) as Sprite;
        //if cards are found in the project
        if (cards.Length > 0)
        {
            //create the dropdown 
            generateSelectedIndex = EditorGUILayout.Popup("Choose a card :", generateSelectedIndex, cardNames);
        }
        else
        {
            GUILayout.Label("No cards found.", EditorStyles.boldLabel);
        }

        GUILayout.EndVertical();

        if (GUILayout.Button("Generate Card"))
        {
            GenerateCard();
            Debug.Log("Card Generated Successfully");
        }

        //introducing delete
        GUILayout.Space(15);
        GUILayout.Label("Delete A Card", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("If you just created a card and it doesn't show,\nPress 'Reload Cards'", MessageType.Info);
        GUILayout.Space(5);

        //if cards are found in the project
        if (cards.Length > 0)
        {
            //create the dropdown 
            deleteSelectedIndex = EditorGUILayout.Popup("Choose a card :", deleteSelectedIndex, cardNames);

            GUILayout.Space(5);

            //Delete button that will also save the asset change in memory and reload the dropdown
            if (GUILayout.Button("Delete Card"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cards[deleteSelectedIndex]));
                Debug.Log("Card Deleted");
                AssetDatabase.SaveAssets();
                Debug.Log("Saved");
                LoadCards();
            }
        }
        else
        {
            GUILayout.Label("No cards found.", EditorStyles.boldLabel);
        }
        //reloads cards if a new card was created without the generation window
        if(GUILayout.Button("Reload Cards"))
        {
            LoadCards();
        }

    }


}
