using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine.Rendering;
using log4net.Core;
using System;
using Unity.VisualScripting.FullSerializer;
using System.Collections.Generic;

public class CardEditor : EditorWindow
{
    private string generatedCardName;
    private Sprite cardImage;

    private int generateSelectedIndex = 0;

    private System.Type[] cardTypes;
    //Delete Dropdown menu 
    private int deleteSelectedIndex = 0;
    private CardBase[] cards;
    private string[] classCards;
    private string[] cardNames;
    private int classSelectedIndex = 0;

    [MenuItem("Window/Card Generator")]
    public static void ShowWindow()
    {
        GetWindow<CardEditor>("Card Generator");
    }

    private void OnEnable()
    {
        LoadCards();
        LoadCardClass();
    }

    void LoadCards()
    {
        string[] guids = AssetDatabase.FindAssets("t:CardBase");
        cards = guids.Select(guid => AssetDatabase.LoadAssetAtPath<CardBase>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        cardNames = cards.Select(card => card.name).ToArray();

    }

    void LoadCardClass()
    {
        //load class
        cardTypes = ReflectionUtils.GetClassesOf<CardBase>();
        Debug.Log($"Result: {cardTypes}");
        classCards = cardTypes.Select(c => c.Name).ToArray();

    }

    void GenerateCard()
    {
        LoadCards();

    }

    void OnGUI ()
    {
        GUILayout.Label("Generate your dream card!", EditorStyles.boldLabel);
        
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.Label("Card Parameters", EditorStyles.boldLabel);
        GUILayout.Space(5);

        generatedCardName = EditorGUILayout.TextField("Name", generatedCardName);

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image");
        cardImage = EditorGUILayout.ObjectField(cardImage, typeof(Sprite), false) as Sprite;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (classCards.Length > 0)
        {
            GUILayout.Label("Chose A Class");
            classSelectedIndex = EditorGUILayout.Popup("", classSelectedIndex, classCards);
        }
        else
        {
            GUILayout.Label("No cards found.", EditorStyles.boldLabel);
        }

        if (GUILayout.Button("Generate Card", GUILayout.Width(150)))
        {
            GenerateCard();
            Debug.Log("Card Generated Successfully");
        }
        GUILayout.EndHorizontal();

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


        GUILayout.Label("Test Dropdown");

    }


}
