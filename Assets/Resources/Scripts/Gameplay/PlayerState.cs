using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] Dictionary<GameItemObject, int> inventory;

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
