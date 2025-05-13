using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CraftSystem : MonoBehaviour
{
    [Header("PlayerState Reference")]
    [SerializeField] PlayerState playerState;

    [Header("Prefabs")]
    [SerializeField] GameObject itemSpritePrefab;
    [SerializeField] GameObject recipeButtonPrefab;

    [Header("UI References")]
    [SerializeField] GameObject craftingUI;
    [SerializeField] Transform gameItemContainer;
    [SerializeField] Transform recipeListContainer;
    [SerializeField] Transform patternContainer;
    [SerializeField] Transform craftingUICanva;
    [SerializeField] Button removePartsButton;

    [Header("Scriptable Object References")]
    [SerializeField] GameItemObject poutre;
    [SerializeField] CraftDataObject clayPowder;
    [SerializeField] CraftDataObject poutreCraft;


    List<GameObject> pieceList = new List<GameObject>();
    List<CraftDataObject> craftList;
    List<GameObject> gameItemList = new List<GameObject>();
    CraftDataObject selectedCraft;
    GameItemObject draggedGI;
    GameObject currentPiece = null;
    GameObject piece;
    GameObject activePattern = null;
    bool displayState = true;


    private void Start()
    {
        playerState.ModifyQuantity(poutre, 2);
        RefreshCraftList();
        RefreshGameItemList();
        SwitchItemDisplay(displayState);
        removePartsButton.GetComponent<Button>().onClick.AddListener(() => { RemoveAllParts(); });
    }

    private void Update()
    {
        if (currentPiece)
        {
            currentPiece.transform.position = Input.mousePosition;
            //Debug.Log(Input.mousePosition);
        }
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

    public void RefreshGameItemList()
    {
        if (gameItemList.Count != 0)
        {
            foreach (var gi in gameItemList)
            {
                Destroy(gi);
            }
            gameItemList.Clear();
        }

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

            #region Adding Event Triggers

            EventTrigger eventTrigger = prefab.transform.GetComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { BeginDrag(j); });
            eventTrigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { EndDrag(); });
            eventTrigger.triggers.Add(pointerUpEntry);

            #endregion

            gameItemList.Add(prefab);
        }
        SwitchItemDisplay(displayState);
    }

    public void SwitchItemDisplay(bool state)
    {
        for(int i = 0; i < gameItemContainer.childCount; i++)
        {
            Transform child = gameItemContainer.GetChild(i);
            child.Find("ItemIcon").gameObject.SetActive(state);
            child.Find("PuzzleIcon").gameObject.SetActive(!state);

        }

        displayState = !displayState;
    }

    public void SelectCraft(int index)
    {
        //Debug.Log($"{index}");

        selectedCraft = craftList[index];
        DisplayCraft();
    }

    public void DisplayCraft()
    {
        if (activePattern) { Destroy(activePattern); }

        activePattern = Instantiate(selectedCraft.pattern, parent:patternContainer);
    }

    public void BeginDrag(int index)
    {
        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = false;

        draggedGI = playerState.inventory.Keys.ToList()[index];

        int quant = playerState.inventory[draggedGI];

        if (quant != 0)
        {
            piece = Instantiate(draggedGI.piece, parent: craftingUICanva);
            
            currentPiece = piece;
            pieceList.Add(piece);
        }
        else
        {
            Debug.Log($"Not Enough {draggedGI.name}");
        }
    }

    public void EndDrag()
    {
        if (currentPiece)
        {
            //check for position
            if (IsInRange(currentPiece, 255, 815, 100, 475))
            {
                playerState.ModifyQuantity(draggedGI, -1);
                RefreshGameItemList();

                Transform nearestJoint = GetNearestJoint(currentPiece.transform);
                Debug.Log(nearestJoint);
                if (nearestJoint == null) currentPiece = null;
                else Place(currentPiece.transform, nearestJoint);
            }
            else Destroy(piece);
        }
        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = true;
    }

    public bool IsInRange(GameObject piece, float minX, float maxX, float minY, float maxY)
    {
        return piece.transform.position.x > minX
           && piece.transform.position.x < maxX
           && piece.transform.position.y > minY
           && piece.transform.position.y < maxY;
    }

    public Transform GetNearestJoint(Transform piece)
    {
        float distance;
        float nearestDistance = math.INFINITY;
        Transform nearestJoint = piece;

        if (activePattern)
        {
            for (int i = 0; i < activePattern.transform.childCount; i++)
            {
                distance = (activePattern.transform.GetChild(i).position - piece.position).magnitude;
                if (nearestDistance < distance)
                {
                    nearestDistance = distance;
                    nearestJoint = activePattern.transform.GetChild(i);
                }
            }
        }
        else nearestJoint = null;

        if (nearestJoint != null && (nearestJoint.position - piece.position).magnitude < 10f) return nearestJoint;
        else return null;

    }

    public void Place(Transform piece, Transform at)
    {
        piece.transform.position = at.position;
    }

    public void RemoveAllParts()
    {
        Debug.Log($"Remove Parts Triggered");

        foreach (var p in pieceList)
        {
            playerState.ModifyQuantity(, 1);
            Destroy(p);
        }
        pieceList.Clear();
    }

    public bool PatternIsValid(List<Sprite> pieces)
    {
        for (int i = 0; i < activePattern.transform.childCount; i++)
        {

        }



        return true;
    }

    public void Craft()
    {

    }


}
