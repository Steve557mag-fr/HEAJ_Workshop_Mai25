using System.Collections.Generic;
using System.Text.RegularExpressions;
using Articy.Unity;
using Articy.Test;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

enum NarrationState{
    DIALOG,
    CHOICE,
    CLOSED
}

[System.Serializable]
struct CharacterState
{
    public string characterName;
    public string characterState;

    [ArticyTypeConstraint(typeof(Hub))]
    public ArticyRef characterReference;

}


[RequireComponent(typeof(ArticyFlowPlayer))]
public class NarrationSystem : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [Header("References")]
    [SerializeField] ArticyFlowPlayer flowPlayer;

    [Header("Characters")]
    [SerializeField] CharacterState[] characters;

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
        actionsFlowFragement.Add("play_audio", (args) => { SoundManager.Get().PlayAt(args[0]); });
        actionsFlowFragement.Add("show_ui", ExecShowUI);
        actionsFlowFragement.Add("hide_ui", ExecHideUI);
        actionsFlowFragement.Add("add_item", ExecAddItem);
        actionsFlowFragement.Add("add_hint", ExecAddHint);
        actionsFlowFragement.Add("set_state", (args) => { NarrationSystem.Get().SetCharacterState(args[0], args[1]); });
        actionsFlowFragement.Add("set_active", (args) => { DataSystem.Get().SetData($"{args[0]}_{args[1]}_enabled", Boolean.Parse(args[2])); });
    }

    private void ExecAddHint(string[] obj)
    {
        InventoryItemObject hi = HintObject.fromString(obj[0]);
        if (hi == null) return;
        if (!(hi is HintObject)) return;

        PlayerState.Get().ModifyQuantity(hi, int.Parse(obj[1]));
    }
    private void ExecAddItem(string[] obj)
    {
        InventoryItemObject gi = GameItemObject.fromString(obj[0]);
        if (gi == null) return;
        if (!(gi is GameItemObject)) return;

        PlayerState.Get().ModifyQuantity((GameItemObject)gi, int.Parse(obj[1]));
    }

    private void ExecHideUI(string[] obj)
    {
        UIManager.Get().SetUI(obj[0], false);
    }
    private void ExecShowUI(string[] obj)
    {
        UIManager.Get().SetUI(obj[0], true);
    }

    public void StartWith(ArticyObject node)
    {
        var list = ArticyFlowPlayer.GetBranchesOfNode(node);
        flowPlayer.Play(list[0]);
    }
    public void StartWith(string rawCharacter)
    {
        print($"[ARTICY]: character to find >> {rawCharacter}");

        foreach (var chr in characters)
        {
            if (chr.characterName != rawCharacter) break;

            Hub hub = chr.characterReference.GetObject<Hub>();
            var branches = ArticyFlowPlayer.GetBranchesOfNode(hub);
            var conx = hub.OutputPins[0].Connections;
            for (int i = 0; i < conx.Count; i++)
            {
                var con = conx[i];
                var branch = branches[i];
                print($"branch:{branches.Count}");
                print($"con:{con.Label} ; branch:{branch.DefaultDescription}");
                if (con.Label.ToString().Replace("STATE_", "") == characters[i].characterState)
                {
                    flowPlayer.Play(branch);
                    return;
                }
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
        for (int i = 0; i < branchindex.Length; i++)
        {
            Branch aBranch = aBranches[i];
            branchindex[i] = aBranch.BranchId;
        }

    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if (aObject == null) return;
        print($"[ARTICY]: new node: {aObject.GetType()}");

        if (aObject.GetType() == typeof(Hub)) DisplayChoices(aObject as Hub);
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

    void ToggleDialogUI(bool enabled = false)
    {
        dialogContainer.SetActive(enabled);
    }

    void ToggleChoiceUI(bool enabled = false)
    {
        choicesContainer.SetActive(enabled);
    }

    void DisplayChoices(Hub choice)
    {
        print("[ARTICY]: update dialog!");
        ToggleChoiceUI(true);
        state = NarrationState.CHOICE;
        List<OutgoingConnection> connections = choice.OutputPins[0].Connections;

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            bool b = i < connections.Count;
            choiceButtons[i].SetActive(b);
            choiceTexts[i].text = b ? connections[i].Label : "";
        }
    }

    void DisplayDialog(DialogueFragment dialog)
    {
        print("[ARTICY]: update diag");
        ToggleDialogUI(true);
        state = NarrationState.DIALOG;

        uiTextDialog.text = dialog.Text;
        uiTextDisplayName.text = dialog.Speaker != null ? dialog.Speaker.name : "? ? ?";

    }

    void DispatchEvent(FlowFragment flow)
    {
        print("[ARTICY]: dispatch event");
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
