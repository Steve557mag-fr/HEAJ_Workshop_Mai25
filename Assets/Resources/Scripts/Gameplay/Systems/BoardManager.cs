using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour, IDataHandle
{
    [Header("References")]
    [SerializeField] CanvasGroup fadingGroup;

    [Header("Transition")]
    [SerializeField] float fadingTime = 1;
    [SerializeField] LeanTweenType easeMode;

    [Header("Usages")]
    [SerializeField] string entryBoard;

    public delegate void OnBoardLoaded(string boardLoaded);
    internal OnBoardLoaded boardLoaded;

    internal string currentBoardName = "";
    bool isLock = false;

    private void Start()
    {
        boardLoaded += WhenBoardLoaded;
        if(entryBoard != "") LoadBoard(entryBoard); // load by default
    }

    // TODO : reinforce the safety about the player capacity 
    // between transition (block interactions)
    public void LoadBoard(string boardName)
    {
        if (isLock) return;
        isLock = true;

        //1. fade out
        LeanTween.alphaCanvas(fadingGroup, 1, fadingTime).setOnComplete(() => {
            //2a. unload current board
            if(currentBoardName != "") SceneManager.UnloadSceneAsync(currentBoardName);
        
            //2b. load new board
            SceneManager.LoadScene(boardName, LoadSceneMode.Additive);
            currentBoardName = boardName;

            //3. fade in
            LeanTween.alphaCanvas(fadingGroup, 0, fadingTime).setEase(easeMode).setOnComplete(() => {
                boardLoaded(currentBoardName);
            }).setDelay(fadingTime/2f);

        }).setEase(easeMode);
    }

    void WhenBoardLoaded(string boardName)
    {
        isLock = false;

    }

    public static void SetClickablesActive(bool state = false, string filter = "")
    {
        foreach(var clk in FindObjectsByType<CickableInteraction>(FindObjectsSortMode.InstanceID))
        {
            if(clk.CompareTag(filter) || filter == "") clk.gameObject.SetActive(state);
        }
    }

    internal static BoardManager Get()
    {
        return FindFirstObjectByType<BoardManager>();
    }

    public JObject toJObject()
    {
        throw new System.NotImplementedException();
    }

    public void fromJObject(JObject jo)
    {
        throw new System.NotImplementedException();
    }

    public JObject getDefaultJObject()
    {
        return new()
        {
            {"board", ""}
        };
    }
}
