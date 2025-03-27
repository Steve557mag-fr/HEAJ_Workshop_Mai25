using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine.Rendering;
using log4net.Core;

public class CardEditor : EditorWindow
{
    //Dropdown menu 
    private int selectedIndex = 0;
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

    void OnGUI ()
    {
        GUILayout.Label("Generate your dream card!", EditorStyles.boldLabel);
        
        GUILayout.BeginVertical();
        GUILayout.Label("Stats", EditorStyles.boldLabel);


        GUILayout.EndVertical();

        if (GUILayout.Button("Generate Card"))
        {
            Debug.Log("Card Generated Successfully");
        }

        GUILayout.Space(15);
        GUILayout.Label("Delete A Card", EditorStyles.boldLabel);
        GUILayout.Label("If you just created a card and it doesn't show,\nPress 'Reload Cards'");
        GUILayout.Space(5);

        if (cards.Length > 0)
        {
            selectedIndex = EditorGUILayout.Popup("Choose a card :", selectedIndex, cardNames);

            GUILayout.Space(5);

            if (GUILayout.Button("Delete Card"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cards[selectedIndex]));
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
        if(GUILayout.Button("Reload Cards"))
        {
            LoadCards();
        }

    }


}
