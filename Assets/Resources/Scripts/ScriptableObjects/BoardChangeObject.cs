using UnityEngine;

[CreateAssetMenu(fileName = "BoardChangeObject", menuName = "")]
public class BoardChangeObject : ScriptableObject
{

    [SerializeField] string boardName;

    public void Run()
    {
        Player.Get().inNavigation = false;
        BoardManager.Get().LoadBoard(boardName);
    }

}
