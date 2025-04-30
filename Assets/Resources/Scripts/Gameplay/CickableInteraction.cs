using Unity.VisualScripting;
using UnityEngine;

public class CickableInteraction : BaseInteraction
{

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown Triggered");
        Trigger();
    }

    
}
