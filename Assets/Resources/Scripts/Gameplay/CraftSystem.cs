using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField] Transform craftingUICanva;
    [SerializeField] GameItemObject poutre;
    [SerializeField] CraftDataObject clayPowder;
    [SerializeField] CraftDataObject poutreCraft;


    CraftDataObject selectedCraft;
    List<CraftDataObject> craftList;
    List<GameObject> pieceList = new List<GameObject>();
    GameObject pattern;
    GameObject currentPiece;


    private void Start()
    {
        playerState.ModifyQuantity(poutre, 2);
        RefreshCraftList();
        RefrehGameItemList();


    }

    private void Update()
    {
        if (currentPiece) currentPiece.transform.position = Input.mousePosition;
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
        var key = playerState.inventory.Keys.ToList();

        for (int i = 0; i < playerState.inventory.Keys.Count; i++)
        {
            var gi = key[i];

            int j = i;

            int quantity = playerState.inventory[gi];
            string name = gi.name;
            Sprite itemIcon = gi.itemIcon;
            Sprite puzzleIcon = gi.puzzleIcon;

            Debug.Log($"Quantity = {quantity}, name = {name}, ");

            GameObject prefab = Instantiate(itemSpritePrefab, parent: gameItemContainer);
            prefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            
            prefab.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = (quantity == -1) ? " " : quantity.ToString();

            prefab.transform.Find("ItemIcon").GetComponent<Image>().sprite = itemIcon;
            prefab.transform.Find("PuzzleIcon").GetComponent<Image>().sprite = puzzleIcon;

            EventTrigger eventTrigger = prefab.transform.GetComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { BeginDrag(j); });
            eventTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { EndDrag(); });
            eventTrigger.triggers.Add(pointerUpEntry);
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

        Debug.Log($"Begun Drag");
        currentPiece = Instantiate(playerState.inventory.Keys.ToList()[index].piece, parent:craftingUICanva);

        pieceList.Add(currentPiece);
        
    }

    public void EndDrag()
    {
        Debug.Log($"Ended Drag");
        if (currentPiece) pieceList.Remove(currentPiece); Destroy(currentPiece);
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
