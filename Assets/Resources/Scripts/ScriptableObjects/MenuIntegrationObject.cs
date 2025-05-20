using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "MenuIntegrationObject", menuName = "MenuIntegration Object")]
public class MenuIntegrationObject : ScriptableObject
{
    
    public void RunNewGame()
    {
        SceneManager.LoadScene("BaseGameplay");

    }

    public void RunContinue()
    {

    }

    public void RunQuit()
    {
        Application.Quit();
    }

}
