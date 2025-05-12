using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CraftSystem : MonoBehaviour
{
    [SerializeField] GameObject craftingUI;
    [SerializeField] PlayerState playerState;
    [SerializeField] GameObject itemSpritePrefab;
    [SerializeField] GameObject recipeButtonPrefab;
    [SerializeField] Transform gameItemContainer;
    [SerializeField] Transform recipeListContainer;
    [SerializeField] Transform patternContainer;
    [SerializeField] GameItemObject poutre;
    [SerializeField] CraftDataObject clayPowder;
    [SerializeField] CraftDataObject poutreCraft;


    CraftDataObject selectedCraft;
    List<CraftDataObject> craftList;
    List<GameObject> pieceList;
    GameObject pattern;
 
    private void Start()
    {
        playerState.ModifyQuantity(poutre, 2);
        RefreshCraftList();
        RefrehGameItemList();
    }

    internal static CraftSystem Get()
    {
        return FindFirstObjectByType<CraftSystem>();
    }

    public void SetUIActive(bool state)
    {
        craftingUI.SetActive(state);
    }

    public void RefreshCraftList()
    {
        craftList = GameManager.Get().QueryAvailableCrafts();

        craftList.Add(clayPowder);
        craftList.Add(poutreCraft);

        for (int i = 0; i < craftList.Count; i++)
        {
            string name = craftList[i].name;
            GameItemObject result = craftList[i].craftResult;
            GameObject pattern = craftList[i].pattern;
            int j = i;
            Debug.Log($"name = {name}, result = {result.name}");

            GameObject prefab = Instantiate(recipeButtonPrefab, parent: recipeListContainer); 
            prefab.transform.Find("Button").Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            prefab.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { SelectCraft(j); }); 
        }
    }

    public void RefrehGameItemList()
    {
        foreach (var k in playerState.inventory.Keys)
        {
            int quantity = playerState.inventory[k];
            string name = k.name;
            Sprite itemIcon = k.itemIcon;
            Sprite puzzleIcon = k.puzzleIcon;

            Debug.Log($"Quantity = {quantity}, name = {name}, ");

            GameObject prefab = Instantiate(itemSpritePrefab, parent: gameItemContainer);
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
        Debug.Log($"{index}");


        selectedCraft = craftList[index];
        DisplayCraft();
    }

    public void DisplayCraft()
    {
        if (pattern) { Destroy(pattern); }

        pattern = Instantiate(selectedCraft.pattern, parent:patternContainer);
    }

    public void RemoveAllParts()
    {

    }

    public void Craft()
    {

    }

    public void BeginDrag(int index)
    {
        GameObject piece = Instantiate(playerState.inventory.Keys.ToList()[index].piece, parent: patternContainer);

        pieceList.Add(piece);
        
    }

    public void EndDrag()
    { 
        
    }

    public Vector3 GetNearestJoint(Transform part)
    {
        float distance;
        float nearestDistance = math.INFINITY;
        Vector3 nearestJoint = part.position;

        for (int i = 0; i < pattern.transform.childCount; i++)
        {
            distance = (pattern.transform.GetChild(i).position - part.position).magnitude;
            if (nearestDistance < distance)
            {
                nearestDistance = distance;
                nearestJoint = pattern.transform.GetChild(i).position;
            }
        }

        return nearestJoint;
    }

    public bool PatternIsValid(List<Sprite> parts)
    {
        for (int i = 0; i < pattern.transform.childCount; i++)
        {

        }



        return true;
    }

    public void Place(Sprite part, Transform at)
    {

    }
}
