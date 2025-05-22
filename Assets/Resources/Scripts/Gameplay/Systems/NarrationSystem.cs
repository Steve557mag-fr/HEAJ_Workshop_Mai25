using System.Collections.Generic;
using System.Text.RegularExpressions;
using Articy.Unity;
using Articy.Test;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEditor.Experimental.GraphView;
using Newtonsoft.Json.Linq;


[System.Serializable]
struct CharacterInfo
{
    public string characterName;
    public string characterState;

    [ArticyTypeConstraint(typeof(Hub))]
    public ArticyRef characterReference;

    public AudioResource clipTalk;

}
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
    [SerializeField] CharacterInfo[] characters;

    [Header("UI References")]
    [SerializeField] List<TextMeshProUGUI> choiceTexts;
    [SerializeField] List<GameObject> choiceButtons;
    [SerializeField] GameObject choicesContainer, dialogContainer;
    [SerializeField] TextMeshProUGUI uiTextDialog, uiTextDisplayName;

    List<int> branchindex = new();
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
        actionsFlowFragement.Add("collect", (args) => { CollectItemObject.TriggerEndCollect(); });
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

        PlayerState.Get().ModifyQuantity((GameItemObject)gi, obj.Length > 1 ? int.Parse(obj[1]) : 1);
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
        flowPlayer.StartOn = node;
        flowPlayer.Play();
    }
    public void StartWith(string rawCharacter)
    {
        print($"[ARTICY]: character to find >> {rawCharacter}");

        CharacterInfo? chr = GetCharacterInfo(rawCharacter);
        if (chr.HasValue) StartWithHub(chr.Value.characterReference.GetObject<Hub>(), chr.Value.characterState);
        else Debug.LogWarning("character not found!");

    }
    public void StartWithHub(Hub hub, string state)
    {
        print($"hub to check: {hub}");
        print($"hub.OutputPins.Count : {hub.OutputPins.Count}");
        if (hub.OutputPins.Count == 0) return;
        
        // get branches
        var branches = ArticyFlowPlayer.GetBranchesOfNode(hub);
        print($"branch:{branches.Count}");

        // Check each connections
        var conx = hub.OutputPins[0].Connections;
        print($"connexions.Count :{conx.Count}");


        for (int i = 0; i < conx.Count; i++)
        {
            var con = conx[i];
            var branch = branches[i];
            print($"con:{con.Label} ; branch:{branch.DefaultDescription}");
            
            if (con.Label == state)
            {
                print("> conx find!");
                flowPlayer.Play(branch);
                return;
            }
        }

    }
    public void NextDialog()
    {
        if (state == NarrationState.CHOICE) return;
        Next();
    }
    public void ChooseChoice(int index)
    {
        if (state == NarrationState.DIALOG) return;
        state = NarrationState.DIALOG;
        ToggleChoiceUI();
        Next(index);
    }
    void Next(int index = 0)
    {
        if (index < 0 && index >= branchindex.Count) return;
        print($"[ARTICY]: next node (branches={branchindex.Count})");
        flowPlayer.Play(branchindex.Count == 0 ? 0 : branchindex[index]);   
    }


    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        print($"[ARTICY]: branches availables: {aBranches.Count}");

        branchindex = new(aBranches.Count);
        for (int i = 0; i < branchindex.Count; i++)
        {
            Branch aBranch = aBranches[i];
            branchindex[i] = aBranch.BranchId;
        }

        if (!flowPlayer.CurrentObject.HasReference)
        {
            Close();
            return;
        }
        print($"[ARTICY]: current object ({flowPlayer.CurrentObject.GetObject().TechnicalName})");

    }
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if (aObject == null) return;
        print($"[ARTICY]: new node: {aObject.GetType()}");

        if (aObject.GetType() == typeof(Hub)) DisplayChoices(aObject as Hub);
        else if (aObject.GetType() == typeof(DialogueFragment)) DisplayDialog(aObject as DialogueFragment);
        else if (aObject.GetType() == typeof(FlowFragment)) DispatchEvent(aObject as FlowFragment);
        else if(aObject.GetType() == typeof(Dialogue))
        {
            Dialogue d = aObject as Dialogue;
            print($"dialogue : {d.DisplayName}");
            StartWith(d.Children.First());
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
        uiTextDisplayName.text = dialog.Speaker != null ? ArticyDatabase.GetObject<Entity>(dialog.Speaker.Id).DisplayName : "? ? ?";

        CharacterInfo? chr = GetCharacterInfo(uiTextDisplayName.text);
        if (chr.HasValue) SoundManager.Get().PlayDialog(chr.Value.clipTalk);

    }
    void DispatchEvent(FlowFragment flow)
    {
        print($"[ARTICY]: dispatch event >> \n{flow.Text}");
        string stringAuto = Regex.Replace(flow.Text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        print(stringAuto);
        var args = stringAuto.Split('\n').ToList();

        string method = args[0];
        args.RemoveAt(0);

        if (actionsFlowFragement.ContainsKey(method))
        {
            actionsFlowFragement[method].Invoke(args.ToArray());
        }
        Next();
    }
    void Close()
    {
        ToggleUI(false);
        state = NarrationState.CLOSED;
    }


    internal CharacterInfo? GetCharacterInfo(string character)
    {
        foreach(var chr in characters)
        {
            if(chr.characterName == character)
            {
                return chr;
            }
        }
        return null;
    }
    internal void SetCharacterState(string character, string characterState)
    {
        for(int i = 0; i < characters.Length; i++)
        {
            var chr = characters[i];

            if (chr.characterName == character)
            {
                chr.characterState = characterState;
            }
        }
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
