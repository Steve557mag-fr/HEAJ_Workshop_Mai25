using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ClickableInteraction : BaseInteraction
{

    [SerializeField] UnityEvent onEnter;
    [SerializeField] UnityEvent onExit;
    [SerializeField] GameObject container;
    internal void SetActivation(bool state)
    {
        container.SetActive(state);
    }

    private void OnMouseDown()
    {
        if (BoardManager.Get().IsBusy() || CraftSystem.Get().IsBusy() || NarrationSystem.GetNarrationState() != NarrationState.CLOSED) return;

        Debug.Log("ClickableInteraction :: OnMouseDown Triggered");
        Trigger();
    }


    private void OnMouseEnter()
    {
        onEnter?.Invoke();
    }

    private void OnMouseExit()
    {
        onExit?.Invoke();
    }

}
