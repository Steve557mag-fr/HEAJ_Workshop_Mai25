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


public class NarrationSystem : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    [SerializeField] ArticyRef refTestOnStart;

    [Header("References")]
    [SerializeField] ArticyFlowPlayer flowPlayer;

    [Header("UI References")]
    [SerializeField] List<TextMeshProUGUI> choiceTexts;
    [SerializeField] GameObject choicesContainer, dialogContainer;
    [SerializeField] TextMeshProUGUI uiTextDialog, uiTextDisplayName;

    NarrationState state;

    private void Start()
    {
        state = NarrationState.CLOSED;
        NextDialog();
    }

    public void NextDialog()
    {
        if (state == NarrationState.CHOICE) return;
        Next();
    }

    public void ChooseBranch(int index)
    {
        if (state == NarrationState.DIALOG) return;
        ToggleChoiceUI();
        state = NarrationState.DIALOG;
        Next(index);
    }

    void Next(int index = 0)
    {
        flowPlayer.Play(index);
    }

    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        Debug.Log(aBranches.Count);
        if (aBranches.Count == 0) return;

        Debug.Log(aBranches[0]);
        Debug.Log(aBranches[0].IsValid);
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
        for(int i = 0; i < connections.Count; i++)
        {
            choiceTexts[i].text = connections[i].Label;
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

}
