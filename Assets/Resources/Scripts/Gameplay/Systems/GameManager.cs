using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<CraftDataObject> QueryAvailableCrafts()
    {
        return new();
    }

    public static GameManager Get()
    {
        return FindFirstObjectByType<GameManager>();
    }

    public void Pause()
    {
        Time.timeScale = 1 - Time.timeScale;
        
    }

    public void LoadGame(string slotName)
    {
        
    }

    public void SaveGame(string slotName) {

        JObject saveSlot = new();
        saveSlot.Add("board", BoardManager.Get().toJObject());
        saveSlot.Add("playerState", PlayerState.Get().toJObject());
        //saveSlot.Add("playerState", 0); /*.Get().toJObject()*/


        DataSystem.Get().SetData(slotName, saveSlot);

    }

    public void Quit()
    {
        Application.Quit();
    }

}
