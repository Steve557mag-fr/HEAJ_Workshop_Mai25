using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{

    [SerializeField] Dictionary<string, GameObject> containers;

    internal void SetUI(string name, bool state = false)
    {
        if (containers.ContainsKey(name)) containers[name].SetActive(state);
    }

    public void ShowUI(string name)
    {
        SetUI(name, false);
    }

    public void HideUI(string name)
    {
        SetUI(name, false);
    }

    static internal UIManager Get()
    {
        return FindAnyObjectByType<UIManager>();
    }

}
