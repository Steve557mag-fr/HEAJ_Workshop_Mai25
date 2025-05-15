using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    internal bool inNavigation = false;

    private void Awake()
    {
        BoardManager.Get().boardLoaded += StartBoard;
    }

    void StartBoard(string boardLoaded)
    {
        inNavigation = false;
        BoardManager.Get().SetClickablesActive(false, "tp");

    }

    public void ToggleUINavigation()
    {
        inNavigation = !inNavigation;
        print(inNavigation);
        BoardManager.Get().SetClickablesActive(inNavigation, "tp");
        BoardManager.Get().SetClickablesActive(!inNavigation, "dia");
    }

    internal static Player Get()
    {
        return FindAnyObjectByType<Player>();
    }
}

