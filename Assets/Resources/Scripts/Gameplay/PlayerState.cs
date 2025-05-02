using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] internal Dictionary<GameItemObject, int> inventory = new();

    public void ModifyQuantity(GameItemObject item, int quantity = 1, bool markInfinite = false)
    {
        if (inventory.ContainsKey(item))
        {
            int prevQ = inventory[item];
            inventory.Add(item, markInfinite ? -1 : prevQ + quantity);
        }
        else inventory.Add(item, markInfinite ? -1 : quantity);
    }
}
