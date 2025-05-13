using System.Collections.Generic;
using Articy.Test;
using Articy.Unity;
using UnityEngine;
using TMPro;
using NUnit.Framework;

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
        var list = ArticyFlowPlayer.GetBranchesOfNode(node.GetObject());
        flowPlayer.Play(list[0]);
        print($"branches : {list.Count}");

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
        flowPlayer.Play(branchindex[index]);
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        branchindex = new int[aBranches.Count];
        for(int i = 0; i < branchindex.Length;i++) {
            Branch aBranch = aBranches[i];
            branchindex[i] = aBranch.BranchId;
        }

        print($"branches availables: {branchindex.Length}");
    }

    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        if(aObject == null)
        {
            print("the end!");
            return;
        }
        if (aObject.GetType() == typeof(DialogueFragment)) DisplayDialog(aObject as DialogueFragment);

        print(aObject.GetType());

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

        uiTextDisplayName.text = dialog.Speaker != null ? dialog.Speaker.name : "??";

    }

    void UpdateCharacter3D()
    {

    }

    internal static NarrationSystem Get()
    {
        return FindFirstObjectByType<NarrationSystem>();
    }

}
