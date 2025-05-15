using UnityEngine;

public class InventoryItemObject : ScriptableObject
{

    public static InventoryItemObject fromString(string s)
    {
        return Resources.Load<InventoryItemObject>($"Data/GameItem/{s}");
    }

}
