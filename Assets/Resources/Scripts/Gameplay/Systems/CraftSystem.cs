
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] Button craftButton;

    [Header("Scriptable Object References")]
    [SerializeField] GameItemObject blackInk;
    [SerializeField] GameItemObject stringc;
    [SerializeField] GameItemObject wax;
    [SerializeField] GameItemObject blade;
    [SerializeField] GameItemObject wood;
    [SerializeField] CraftDataObject scissors;
    [SerializeField] CraftDataObject candle;

    [Header("Icon Display State")]
    [SerializeField] bool baseIconState = true;

    [Header("Crafting Screen Size Parameters")]
    [SerializeField][Range(0f, 1f)] float minX;
    [SerializeField][Range(0f, 1f)] float maxX;
    [SerializeField][Range(0f, 1f)] float minY;
    [SerializeField][Range(0f, 1f)] float maxY;


    List<GameObject> gameItemList = new();
    List<GameObject> activePieceList = new();
    List<GameObject> validPieceList = new();
    List<CraftDataObject> craftList;
    CraftDataObject selectedCraft;
    GameItemObject draggedGI;
    GameObject activePattern = null;
    GameObject currentPiece = null;
    GameObject piece;
    Vector3 mouseDistance;
    bool iconState;
    bool giExisted;

    private void Start()
    {
        iconState = baseIconState;
        playerState.ModifyQuantity(blackInk, 10);
        playerState.ModifyQuantity(wax, 10);
        playerState.ModifyQuantity(stringc, 10);
        playerState.ModifyQuantity(blade, 10);
        playerState.ModifyQuantity(wood, 10);
        RefreshCraftList();
        RefreshGameItemList();
        removePartsButton.GetComponent<Button>().onClick.AddListener(() => { RemoveAllParts(); });
    }

    private void Update()
    {
        if (currentPiece)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        currentPiece.gameObject.transform.position = new Vector3(Input.mousePosition.x + mouseDistance.x, Input.mousePosition.y + mouseDistance.y, 0);
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

        craftList.Add(candle);
        craftList.Add(scissors);

        for (int i = 0; i < craftList.Count; i++)
        {
            string name = craftList[i].name;
            GameItemObject result = craftList[i].craftResult;
            GameObject pattern = craftList[i].pattern;
            int j = i;

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
            var gi = (GameItemObject)key[i];

            int j = i;

            int quantity = playerState.inventory[gi];
            string name = gi.name;
            Sprite itemIcon = gi.itemIcon;
            Sprite puzzleIcon = gi.puzzleIcon;

            #region Creating GameItem Prefab and Assigning Variables

            GameObject prefab = Instantiate(itemSpritePrefab, parent: gameItemContainer);
            prefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            
            prefab.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = (quantity == -1) ? " " : quantity.ToString();

            prefab.transform.Find("ItemIcon").GetComponent<Image>().sprite = itemIcon;
            prefab.transform.Find("PuzzleIcon").GetComponent<Image>().sprite = puzzleIcon;

            prefab.transform.Find("ItemIcon").gameObject.SetActive(baseIconState);
            prefab.transform.Find("PuzzleIcon").gameObject.SetActive(!baseIconState);

            #endregion

            if (gi.canBePlaced) 
            {
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
            }
            gameItemList.Add(prefab);
        }
    }

    public void SwitchItemDisplay(bool state)
    {
        for(int i = 0; i < gameItemContainer.childCount; i++)
        {
            Transform child = gameItemContainer.GetChild(i);
            child.Find("ItemIcon").gameObject.SetActive(state);
            child.Find("PuzzleIcon").gameObject.SetActive(!state);

        }

        iconState = !iconState;
    }

    public void SelectCraft(int index)
    {
        if (index >= 0)
        {
            RemoveAllParts();

            selectedCraft = craftList[index];
            craftButton.gameObject.SetActive(true);
            DisplayCraft();


        }
        else
        {
            RemoveAllParts();
            selectedCraft = null;
            DisplayCraft();
        }
    }

    public void DisplayCraft()
    {
        if (activePattern) Destroy(activePattern);

        if (selectedCraft)
        {
            activePattern = Instantiate(selectedCraft.pattern, parent: patternContainer);
            activePattern.name = selectedCraft.name;

        }
    }

    public void BeginDrag(int index)
    {
        giExisted = false;
        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = false;

        draggedGI = (GameItemObject)playerState.GetFilteredBy<GameItemObject>()[index];

        int quant = playerState.inventory[draggedGI];

        if (quant != 0)
        {
            piece = Instantiate(draggedGI.piece, parent: craftingUICanva);
            piece.transform.position = gameItemList[index].transform.position;
             
            GameObject tempPiece = piece;

            #region Adding Event Triggers To Make It Draggable

                EventTrigger eventTrigger = piece.transform.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
                pointerDownEntry.eventID = EventTriggerType.PointerDown;
                pointerDownEntry.callback.AddListener((data) => { BeginDragInstance(tempPiece); });
                eventTrigger.triggers.Add(pointerDownEntry);

                EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
                pointerUpEntry.eventID = EventTriggerType.PointerUp;
                pointerUpEntry.callback.AddListener((data) => { EndDrag(); });
                eventTrigger.triggers.Add(pointerUpEntry);

                #endregion

            piece.name = draggedGI.name;
            currentPiece = piece;
            mouseDistance.x = currentPiece.gameObject.transform.position.x - Input.mousePosition.x;
            mouseDistance.y = currentPiece.gameObject.transform.position.y - Input.mousePosition.y;
            activePieceList.Add(piece);
        }
        else
        {
            print($"Not Enough {draggedGI.name}");
        }
    }

    public void BeginDragInstance(GameObject piece)
    {
        giExisted = true;
        currentPiece = piece;
        mouseDistance.x = currentPiece.gameObject.transform.position.x - Input.mousePosition.x;
        mouseDistance.y = currentPiece.gameObject.transform.position.y - Input.mousePosition.y;
    }

    public void EndDrag()
    {
        if (currentPiece)
        {
            //check for position
            if (IsInRange(currentPiece))
            {
                if(!giExisted)playerState.ModifyQuantity(draggedGI, -1);
                RefreshGameItemList();

                Transform nearestJoint = GetNearestJoint(currentPiece.transform);
                if (nearestJoint == null) currentPiece = null;
                else
                {
                    Place(currentPiece.transform, nearestJoint);
                    validPieceList.Add(currentPiece);
                    currentPiece = null;
                }
            }
            else
            {
                activePieceList.Remove(piece);
                Destroy(piece);
            }
        }
        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = true;
    }

    public bool IsInRange(GameObject piece)
    {
        return piece.transform.position.x / Screen.width > minX
           && piece.transform.position.x / Screen.width < maxX
           && piece.transform.position.y / Screen.width > minY
           && piece.transform.position.y / Screen.width < maxY;
    }

    public Transform GetNearestJoint(Transform piece)
    {
        float distance;
        int nearestDistanceIndex = 0;
        float nearestDistance = math.INFINITY;
        Transform nearestJoint = piece;

        if (activePattern)
        {
            for (int i = 0; i < activePattern.transform.childCount; i++)
            {
                distance = (activePattern.transform.GetChild(i).position - piece.position).magnitude;
                if (distance < nearestDistance && activePattern.transform.GetChild(i).name == piece.name)
                {
                    nearestDistance = distance;
                    nearestDistanceIndex = i;
                    nearestJoint = activePattern.transform.GetChild(i);
                }
            }
            if ((nearestJoint.position - piece.position).magnitude < Screen.width/150) return nearestJoint;
            else return null;
        }
        else return null;

    }

    public void Place(Transform piece, Transform at)
    {
        piece.transform.position = at.position;
        validPieceList.Add(piece.gameObject);
        print($"Placed {piece.transform.name} and added it to validPieceList as {validPieceList.Last()}");

    }

    public void RemoveAllParts()
    {
        if(activePieceList.Count != 0)
        {
            foreach (var p in activePieceList)
            {
                //print(p.name);
                playerState.ModifyQuantity((GameItemObject)playerState.fromString(p.name), 1);
                Destroy(p);
            }
            activePieceList.Clear();
            validPieceList.Clear();
            RefreshGameItemList();
            //print("Removed all Parts");
        }
    }

    public bool PatternIsValid()
    {
        for (int i = 0; i < activePattern.transform.childCount; i++) 
        {
            if (validPieceList.Count > 0)
            {
                print(i);
                print(validPieceList[i + 1]);
                if (!validPieceList[i + 1]) return false;
            }
            else print("nothing in validPieceList");
        }
        return true;
    }

    public void Craft()
    {
        if (PatternIsValid() && selectedCraft)
        {
            print($"Pattern {selectedCraft.pattern.name} is valid, adding {selectedCraft.craftResult} to the inventory");
            playerState.ModifyQuantity(selectedCraft.craftResult, 1);
            RefreshGameItemList();

            SelectCraft(-1);

            craftButton.gameObject.SetActive(false);
        }
        else
        {
            print("Pattern is not valid. try again :)");
        }

    }


}
