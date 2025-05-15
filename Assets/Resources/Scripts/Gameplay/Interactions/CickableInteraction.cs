using Unity.VisualScripting;
using UnityEngine;

public class CickableInteraction : BaseInteraction
{

    private void OnMouseDown()
    {
        if (BoardManager.Get().IsBusy() || NarrationSystem.GetNarrationState() != NarrationState.CLOSED) return;

        Debug.Log("ClickableInteraction :: OnMouseDown Triggered");
        Trigger();
    }

    
}
