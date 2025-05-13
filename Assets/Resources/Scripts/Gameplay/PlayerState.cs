using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PlayerState : MonoBehaviour, IDataHandle
{
    [SerializeField] internal Dictionary<GameItemObject, int> inventory = new();

    public void ModifyQuantity(GameItemObject item, int quantity = 1, bool markInfinite = false)
    {
        if (inventory.ContainsKey(item))
        {
            int prevQ = inventory[item];
            inventory[item] = ( markInfinite ? -1 : prevQ + quantity);
        }
        else inventory.Add(item, markInfinite ? -1 : quantity);
    }


    public static PlayerState Get()
    {
        return FindAnyObjectByType<PlayerState>();
    }
    public JObject toJObject()
    {
        // parse inventory
        JObject data = new();
        var gi = inventory.Keys.ToArray();
        for (int i = 0; i < gi.Length; i++)
        {
            data.Add(gi[i].name, inventory[gi[i]]);
        }
        Debug.Log($"inv_parsed: {data}");

        return new(){
            {"inventory", data}
        };
    }
    public void fromJObject(JObject data)
    {

    }
    public JObject getDefaultJObject()
    {
        return new()
        {

            {"inventory", new JObject(){
                "ClayPowder", 1
            }}

        };
    }
}
