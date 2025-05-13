using UnityEngine;

[CreateAssetMenu(fileName = "BoardChangeObject", menuName = "")]
public class BoardChangeObject : ScriptableObject
{

    [SerializeField] string boardName;

    public void Run()
    {
        BoardManager.Get().LoadBoard(boardName);
    }

}
