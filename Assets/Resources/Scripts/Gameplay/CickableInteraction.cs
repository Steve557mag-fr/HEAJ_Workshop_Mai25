using Unity.VisualScripting;
using UnityEngine;

public class CickableInteraction : BaseInteraction
{

    private void OnMouseDown()
    {
        Debug.Log("ClickableInteraction :: OnMouseDown Triggered");
        Trigger();
    }

    
}
