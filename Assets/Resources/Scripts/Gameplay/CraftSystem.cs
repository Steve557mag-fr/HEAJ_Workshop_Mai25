using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftSystem : MonoBehaviour
{
    [SerializeField] GameObject craftingUI;
    [SerializeField] PlayerState playerState;
    [SerializeField] GameObject ItemSpritePrefab;
    [SerializeField] Transform gameItemContainer;
    [SerializeField] Transform recipeListContainer;
    [SerializeField] GameItemObject poutre;

    private void Start()
    {
        playerState.ModifyQuantity(poutre, 2);
        RefreshCraftList();
        RefrehGameItemList();
    }

    public void SetUIActive(bool state)
    {
        craftingUI.SetActive(state);
    }

    public void RefreshCraftList()
    {
        //List<CraftDataObject> craftList = GameManager.Get().QueryAvailableCrafts();
    }

    public void RefrehGameItemList()
    {
        foreach (var k in playerState.inventory.Keys)
        {
            int quantity = playerState.inventory[k];
            string name = k.name;
            Sprite itemIcon = k.itemIcon;
            Sprite puzzleIcon = k.puzzleIcon;

            Debug.Log($"Qquantity = {quantity}, name = {name}, ");

            GameObject prefab = Instantiate(ItemSpritePrefab);
            prefab.transform.parent = gameItemContainer;
            prefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            
            prefab.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = (quantity == -1) ? " " : quantity.ToString();

            prefab.transform.Find("ItemIcon").GetComponent<Image>().sprite = itemIcon;
            prefab.transform.Find("PuzzleIcon").GetComponent<Image>().sprite = puzzleIcon;

        }
        SwitchItemDisplay(true);
    }

    public void SwitchItemDisplay(bool state)
    {
        for(int i = 0; i < gameItemContainer.childCount; i++)
        {
            Transform child = gameItemContainer.GetChild(i);
            child.Find("ItemIcon").gameObject.SetActive(state);
            child.Find("PuzzleIcon").gameObject.SetActive(!state);

        }
    }

    public void SelectCraft(int index)
    {

    }
}
