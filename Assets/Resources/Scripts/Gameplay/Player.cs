using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

<<<<<<< Updated upstream
    bool inNavigation = false;
=======
    [SerializeField] GameObject uiNavigationContainer;

    public void ToggleUINavigation()
    {
        uiNavigationContainer.SetActive(uiNavigationContainer.activeSelf);
>>>>>>> Stashed changes

    public void ToggleUINavigation()
    {
        inNavigation = !inNavigation;   
    }

    


}
