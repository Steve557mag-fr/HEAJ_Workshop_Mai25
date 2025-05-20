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
    [SerializeField] ModalSystem modalSystem;

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

    [Header("Icon Display State")]
    [SerializeField] bool baseIconState = true;

    [Header("Crafting Screen Size Parameters")]
    [SerializeField][Range(0f, 1f)] float minX;
    [SerializeField][Range(0f, 1f)] float maxX;
    [SerializeField][Range(0f, 1f)] float minY;
    [SerializeField][Range(0f, 1f)] float maxY;

    [Header("List of active pieces")]
    [SerializeField] List<Piece> activePieceList = new();

    List<CraftDataObject> craftList = new();
    List<GameObject> gameItemList = new();
    List<GameObject> activeCraftList = new();
    CraftDataObject selectedCraft = null;
    GameObject activePattern = null;
    Vector3 mouseDistance;
    Piece? currentPiece;
    bool iconState;
    bool giExisted;

    [System.Serializable]
    public struct Piece
    {
        public GameItemObject draggedGI;
        public GameObject prefab;
        public bool isPlaced;
        public bool moves;
        public int index;

        public Piece(GameItemObject gi, GameObject prefab, bool isPlaced, int index) : this()
        {
            this.draggedGI = gi;
            this.prefab = prefab;
            this.isPlaced = isPlaced;
            this.index = index;
        }
    }

    private void Start()
    {
        iconState = baseIconState;
        RefreshCraftList();
        RefreshGameItemList();
        removePartsButton.GetComponent<Button>().onClick.AddListener(() => { RemoveAllParts(true); });
    }

    private void Update()
    {
        //print($"current piece is {currentPiece}");
        if (currentPiece.HasValue && currentPiece.Value.prefab)
        {
            print($"active piece 'moves' state is {currentPiece.Value.moves}");
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        currentPiece.Value.prefab.transform.position = new Vector3(Input.mousePosition.x + mouseDistance.x, Input.mousePosition.y + mouseDistance.y, 0);
    }

    internal static CraftSystem Get()
    {
        return FindFirstObjectByType<CraftSystem>();
    }

    public bool IsOpen()
    {
        return craftingUI.activeSelf;
    }

    public void SetUIActive(bool state)
    {
        craftingUI.SetActive(state);
        RefreshCraftList();
        RefreshGameItemList();

        if(state == false)
        {
            SelectCraft(-1);
            RemoveAllParts(false);
        }
    }

    public void RefreshCraftList()
    {
        craftList.Clear();
        for(int i = 0; i < activeCraftList.Count; i++)
        {
            Destroy(activeCraftList[i]);
        }
        activeCraftList.Clear();


        craftList = playerState.GetFilteredBy<CraftDataObject>().Select(e => (CraftDataObject)e).ToList();

         
        for (int i = 0; i < craftList.Count; i++)
        {
            string name = craftList[i].name;
            GameItemObject result = craftList[i].craftResult;
            GameObject pattern = craftList[i].pattern;
            int j = i;

            GameObject prefab = Instantiate(recipeButtonPrefab, parent: recipeListContainer); 
            prefab.transform.Find("Button").Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            prefab.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { SelectCraft(j); });

            activeCraftList.Add(prefab);
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

        var gameitems = playerState.GetFilteredBy<GameItemObject>().Select(e => (GameItemObject)e).ToList();

        for (int i = 0; i < gameitems.Count; i++)
        {
            GameItemObject gi = (GameItemObject)gameitems[i];
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
            RemoveAllParts(true);
            selectedCraft = craftList[index];
            craftButton.gameObject.SetActive(true);
            DisplayCraft();
        }
        else
        {
            RemoveAllParts(false);
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
        GameItemObject draggedGI = (GameItemObject)playerState.GetFilteredBy<GameItemObject>()[index];

        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = false;
        SoundManager.Get().PlayFX(draggedGI.soundGrab);

        int quant = playerState.inventory[draggedGI];
        if (quant != 0)
        {
            GameObject pieceInstance = Instantiate(draggedGI.piece, parent: craftingUICanva);
            pieceInstance.transform.position = gameItemList[index].transform.position;
            pieceInstance.name = draggedGI.name;

            currentPiece = new Piece(draggedGI, pieceInstance, false, activePieceList.Count);
            Piece tempPiece = currentPiece.Value;
            activePieceList.Add(currentPiece.Value);

            #region Adding Event Triggers To Make It Draggable

            EventTrigger eventTrigger = currentPiece.Value.prefab.GetComponent<EventTrigger>();

                EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
                pointerDownEntry.eventID = EventTriggerType.PointerDown;
                pointerDownEntry.callback.AddListener((data) => { BeginDragInstance(tempPiece); });
                eventTrigger.triggers.Add(pointerDownEntry);

                EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
                pointerUpEntry.eventID = EventTriggerType.PointerUp;
                pointerUpEntry.callback.AddListener((data) => { EndDrag(); });
                eventTrigger.triggers.Add(pointerUpEntry); 

            #endregion

            mouseDistance.x = currentPiece.Value.prefab.transform.position.x - Input.mousePosition.x;
            mouseDistance.y = currentPiece.Value.prefab.transform.position.y - Input.mousePosition.y;
        }
        else print($"Not Enough {draggedGI.name}"); 

    }

    public void BeginDragInstance(Piece draggedPiece)
    {
        currentPiece = draggedPiece;
        print($"Current Piece index is {currentPiece.Value.index}");
        mouseDistance.x = currentPiece.Value.prefab.transform.position.x - Input.mousePosition.x;
        mouseDistance.y = currentPiece.Value.prefab.transform.position.y - Input.mousePosition.y;
    }

    public void EndDrag()
    {
        if (!currentPiece.HasValue) return;

        //activate the scroll
        gameItemContainer.GetComponentInParent<ScrollRect>().enabled = true;

        //check for position
        if (!currentPiece.HasValue || !IsInRange(currentPiece.Value.prefab))
        {
            playerState.ModifyQuantity(currentPiece.Value.draggedGI , 1);
            activePieceList.RemoveAt(currentPiece.Value.index);
            Destroy(currentPiece.Value.prefab);
            return;

        }

        //check if the piece was already instantialized
        if (!currentPiece.HasValue) playerState.ModifyQuantity(currentPiece.Value.draggedGI, -1);

        //refresh ui gameItem
        RefreshGameItemList();

        //get the nearest joint if there's one 
        Transform nearestJoint = GetNearestJoint(currentPiece.Value.prefab.transform);
        if (nearestJoint)
        {
            //place the piece at the nearest joint position
            Place(currentPiece.Value, nearestJoint);

        }

        //play the drop sound
        SoundManager.Get().PlayFX(currentPiece.Value.draggedGI.soundDrop);
        
        //release the piece from the drag
        Piece p = activePieceList[currentPiece.Value.index];
        p.isPlaced = nearestJoint != null;
        activePieceList[currentPiece.Value.index] = p;
        currentPiece = null;

    }

    public bool IsInRange(GameObject currentPiece) 
    {
        return currentPiece.transform.position.x / Screen.width > minX
           && currentPiece.transform.position.x / Screen.width < maxX
           && currentPiece.transform.position.y / Screen.height > minY
           && currentPiece.transform.position.y / Screen.height < maxY;
    }   

    public Transform GetNearestJoint(Transform currentPieceTransform)
    {
        float distance;
        int nearestDistanceIndex;
        float nearestDistance = math.INFINITY;
        Transform nearestJoint = currentPieceTransform;

        if (activePattern)
        {
            for (int i = 0; i < activePattern.transform.childCount; i++)
            {
                distance = (activePattern.transform.GetChild(i).position - currentPieceTransform.position).magnitude;
                if (distance < nearestDistance && activePattern.transform.GetChild(i).name == currentPieceTransform.name)
                {
                    nearestDistance = distance;
                    nearestDistanceIndex = i;
                    nearestJoint = activePattern.transform.GetChild(i);
                }
            }
            if ((nearestJoint.position - currentPieceTransform.position).magnitude < Screen.width/130) return nearestJoint;
            else return null;
        }
        else return null;
    }

    public void Place(Piece toPlace, Transform at) 
    {
        toPlace.prefab.transform.position = at.position;
    }

    public void RemoveAllParts(bool fromCraft)
    {
        if(activePieceList.Count != 0)
        {
            foreach (var p in activePieceList)
            {
                if(fromCraft) playerState.ModifyQuantity((GameItemObject)playerState.fromString(p.prefab.name), 1);
                Destroy(p.prefab);
            } 
            activePieceList.Clear();
            RefreshGameItemList();
        }
    }

    public bool PatternIsValid()
    {
        int temp = 0;
        //print($"There are {activePieceList.Count} pieces active");
        foreach (Piece piece in activePieceList)
        {
            //print($"In the {count} loop, {piece.prefab.name}'s isPlaced state is {piece.isPlaced}");
            if (piece.isPlaced)
            {
                temp++;
                //print($"Because {piece.prefab.name}'s isPlaced state is {piece.isPlaced}, the number of correctly placed pieces is {temp}");
            }
        }
        //print($"At the end of PatternIsValid, {temp} pieces were placed correctly and {activePattern.transform.childCount} needed to be correct");
        return temp == activePattern.transform.childCount;

    }

    public bool IsBusy()
    {
        return craftingUICanva.gameObject.activeSelf;
    }

    public void Craft()
    {
        if (selectedCraft && PatternIsValid())
        {
            modalSystem.OpenUI(selectedCraft.craftResult);
            playerState.ModifyQuantity(selectedCraft.craftResult, 1);
            RefreshGameItemList();

            SelectCraft(-1);
            craftButton.gameObject.SetActive(false);

        }
        else print("Pattern is not valid. try again :)");

    }

    private void OnDrawGizmosSelected()
    {
        var w = 1919/2;
        var h = 1080/2;


        var minWX = w * minX;
        var maxWX = w * maxX;
        
        var minWY = h * minY;
        var maxWY = h * maxY;


        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(new(minWX, -100), new(minWX, 100));
        Gizmos.DrawLine(new(maxWX, -100), new(maxWX, 100));

        Gizmos.DrawLine(new(-100, minWY), new(100, minWY));
        Gizmos.DrawLine(new(-100, maxWY), new(100, maxWY));

    }
    
}