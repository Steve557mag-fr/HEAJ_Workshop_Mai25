using System.Collections.Generic;
using Articy.Test_Project;
using Articy.Unity;
using UnityEngine;
using TMPro;

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

    [Header("UI References")]
    [SerializeField] List<TextMeshProUGUI> choiceTexts;
    [SerializeField] List<GameObject> choiceButtons;
    [SerializeField] GameObject choicesContainer, dialogContainer;
    [SerializeField] TextMeshProUGUI uiTextDialog, uiTextDisplayName;

    int[] branchindex;

    NarrationState state;

    private void Start()
    {
        state = NarrationState.CLOSED;
    }

    public void StartWith(ArticyRef node)
    {
        var a = node.GetObject<Hub>();
        if (a != null)
        {
            var list = ArticyFlowPlayer.GetBranchesOfNode(a);
            flowPlayer.Play(list[0]);
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

    void Next(int index = -1)
    {
        flowPlayer.Play(index == -1 ? 0 : branchindex[index]);
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        branchindex = new int[aBranches.Count];
        for(int i = 0; i < branchindex.Length;i++) {
            Branch aBranch = aBranches[i];
            branchindex[i] = aBranch.BranchId;
        }
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if(aObject == null) return;
        if (aObject.GetType() == typeof(DialogueFragment)) DisplayDialog(aObject as DialogueFragment);
        else if (aObject.GetType() == typeof(Hub)) DisplayChoices(aObject as Hub);
        else { 
            // close UI
            ToggleUI();
            state = NarrationState.CLOSED;
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
        uiTextDisplayName.text = dialog.Speaker.name;
        uiTextDialog.text = dialog.Text;
    }

    void UpdateCharacter3D()
    {

    }

    internal static NarrationSystem Get()
    {
        return FindFirstObjectByType<NarrationSystem>();
    }

}
