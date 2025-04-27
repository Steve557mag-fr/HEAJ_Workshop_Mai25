using System;
using System.Collections.Generic;
using Articy.Test_Project;
using Articy.Unity;
using Articy.Unity.Interfaces;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrationSystem : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    JsonSerializer opt = new JsonSerializer()
    { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore };

    [SerializeField] ArticyRef refTestOnStart;

    [Header("References")]
    [SerializeField] ArticyFlowPlayer flowPlayer;

    [Header("UI References")]
    [SerializeField] GameObject uiContainer;
    [SerializeField] TextMeshProUGUI uiTextDialog;
    [SerializeField] TextMeshProUGUI uiTextDisplayName;

    private void Start()
    {
        Next();
    }

    public void Next()
    {
        flowPlayer.Play();
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        Debug.Log(aBranches.Count);
        if (aBranches.Count == 0) return;

        Debug.Log(aBranches[0]);
        Debug.Log(aBranches[0].IsValid);
        //Debug.Log(Newtonsoft.Json.Linq.JObject.FromObject(aBranches,opt));
        //throw new System.NotImplementedException();
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if(aObject == null) return;
        if(aObject.GetType() == typeof(DialogueFragment)) FillUI(aObject as DialogueFragment);
        else if(aObject.GetType() == typeof(Hub)) FillChoices(aObject as Hub); 

        //Debug.Log(Newtonsoft.Json.Linq.JObject.FromObject(aObject,opt));
        //throw new System.NotImplementedException();
    }

    void FillChoices(Hub choices)
    {
        foreach(var pin in choices.OutputPins)
        {
            Debug.Log($"id> {pin.Id}");
        }
    }

    void FillUI(DialogueFragment dialog)
    {
        uiTextDisplayName.text = dialog.Speaker.name;
        uiTextDialog.text = dialog.Text;
    }

}
