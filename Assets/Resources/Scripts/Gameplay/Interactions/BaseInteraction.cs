using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BaseInteraction : MonoBehaviour
{
    public UnityEvent onTrigger;
     
    public virtual void Trigger()
    {
        onTrigger.Invoke();
    }
}
