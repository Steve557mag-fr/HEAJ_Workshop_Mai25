using Unity.VisualScripting;
using UnityEngine;

public class CickableInteraction : BaseInteraction
{

    private void OnMouseDown()
    {
        //if (NarrationSystem.GetNarrationState() != NarrationState.CLOSED) return;

        Debug.Log("ClickableInteraction :: OnMouseDown Triggered");
        Trigger();
    }

    
}
