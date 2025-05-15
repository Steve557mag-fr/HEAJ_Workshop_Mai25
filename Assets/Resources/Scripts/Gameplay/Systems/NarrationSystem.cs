using System.Collections.Generic;
using Articy.Test;
using Articy.Unity;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
using System.Linq;

enum NarrationState{
    DIALOG,
    CHOICE,
    CLOSED
}

[RequireComponent(typeof(ArticyFlowPlayer))]
public class NarrationSystem : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [Header("References")]
    [SerializeField] ArticyFlowPlayer flowPlayer;

    [Header("Characters")]
    [ArticyTypeConstraint(typeof(Hub))]
    [SerializeField] ArticyRef[] characters;

    [Header("UI References")]
    [SerializeField] List<TextMeshProUGUI> choiceTexts;
    [SerializeField] List<GameObject> choiceButtons;
    [SerializeField] GameObject choicesContainer, dialogContainer;
    [SerializeField] TextMeshProUGUI uiTextDialog, uiTextDisplayName;

    int[] branchindex;
    NarrationState state;
    Dictionary<string, Action<string[]>> actionsFlowFragement = new();

    private void Start()
    {
        state = NarrationState.CLOSED;
        actionsFlowFragement.Add("board_load", (args) => { BoardManager.Get().LoadBoard(args[0]); });
        actionsFlowFragement.Add("play_audio", (args) => { /*SoundManager.Get().Play(args[0]);*/ });
        actionsFlowFragement.Add("show_ui", (args) => { /* [0]=> name_ui ; ... */ });
        actionsFlowFragement.Add("add_item", (args) => { /* [0]=> name_item ; ... */ });
        actionsFlowFragement.Add("add_hint", (args) => { /* [0]=> name_hint ; ... */ });

    }

    public void StartWith(ArticyRef node)
    {
        var list = ArticyFlowPlayer.GetBranchesOfNode(node.GetObject());
        flowPlayer.Play(list[0]);
    }

    public void StartWith(string character)
    {
        for(int i = 0; i < characters.Length; i++)
        {
            print($"[ARTICY]: character_name >> {characters[i]}");
            if (characters[i].GetObject().name != character) continue;
            var output = characters[i].GetObject<Hub>().OutputPins;
            for(int j = 0; j < output.Count; j++)
            {
                print($"conx: {output[j].ToString()}");
            }
        }
    }


    public void NextDialog()
    {
        if (state == NarrationState.CHOICE) return;
        Next();
    }

    public void ChooseBranch(int index)
    {
        if (state == NarrationState.DIALOG) return;
        state = NarrationState.DIALOG;
        ToggleChoiceUI();
        Next(index);
    }

    void Next(int index = 0)
    {
        if (index < 0 && index >= branchindex.Length) return;
        print("[ARTICY]: next node");
        flowPlayer.Play(branchindex[index]);
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        print($"[ARTICY]: branches availables: {aBranches.Count}");

        branchindex = new int[aBranches.Count];
        for(int i = 0; i < branchindex.Length;i++) {
            Branch aBranch = aBranches[i];
            branchindex[i] = aBranch.BranchId;
        }

    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        print($"[ARTICY]: new node: {aObject.GetType()}");

        if (aObject == null) return;
        else if (aObject.GetType() == typeof(Hub)) DisplayChoices(aObject as Hub);
        else if (aObject.GetType() == typeof(DialogueFragment)) DisplayDialog(aObject as DialogueFragment);
        else if (aObject.GetType() == typeof(FlowFragment)) DispatchEvent(aObject as FlowFragment);
        else if (aObject.GetType() == typeof(OutputPin))
        {
            state = NarrationState.CLOSED;
            ToggleUI(false);
        }

    }

    void ToggleUI(bool enabled = false)
    {
        ToggleDialogUI(enabled);
        ToggleChoiceUI(enabled);
    }

    void ToggleDialogUI(bool enabled = false) {
        dialogContainer.SetActive(enabled);
    }

    void ToggleChoiceUI(bool enabled = false) {
        choicesContainer.SetActive(enabled);
    }

    void DisplayChoices(Hub choice)
    {
        ToggleChoiceUI(true);
        state = NarrationState.CHOICE;
        List<OutgoingConnection> connections = choice.OutputPins[0].Connections;

        for(int i = 0; i < choiceButtons.Count; i++)
        {
            bool b = i < connections.Count;
            choiceButtons[i].SetActive(b);
            choiceTexts[i].text = b ? connections[i].Label : "";
        }
    }

    void DisplayDialog(DialogueFragment dialog)
    {
        ToggleDialogUI(true);
        state = NarrationState.DIALOG;

        uiTextDialog.text = dialog.Text;
        uiTextDisplayName.text = dialog.Speaker != null ? dialog.Speaker.name : "? ? ?";

    }

    void DispatchEvent(FlowFragment flow)
    {
        string stringAuto = Regex.Replace(flow.Text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        var args = stringAuto.Split('\n').ToList();
        string method = args[0];
        args.RemoveAt(0);

        if (actionsFlowFragement.ContainsKey(method))
        {
            actionsFlowFragement[method].Invoke(args.ToArray());
        }
        Next();
    }


    internal void SetCharacterState(string character, string characterState)
    {
        ArticyDatabase.DefaultGlobalVariables.SetVariableByString(
            $"{character}", characterState
        );
    }

    internal static NarrationSystem Get()
    {
        return FindFirstObjectByType<NarrationSystem>();
    }

    internal static NarrationState GetNarrationState()
    {
        return Get().state;
    }
}
