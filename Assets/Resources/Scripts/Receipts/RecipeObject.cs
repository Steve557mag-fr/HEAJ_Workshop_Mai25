using UnityEngine;
using System.Collections.Generic;


public class RecipeObject : ScriptableObject
{

    [SerializeField] public List<Item> items;
    [SerializeField] public CardBase craftResult;
    
}

public struct Item
{
    string name;
    int quantity;
}
