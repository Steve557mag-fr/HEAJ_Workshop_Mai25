using UnityEngine;

[CreateAssetMenu(fileName = "DraggableScriptableObject", menuName = "Scriptable Objects/DraggableScriptableObject")]
public class DraggableScriptableObject : ScriptableObject
{
    
    public void BeginDrag(int i)
    {
        CraftSystem.Get().BeginDrag(i);
    }

    public void EndDrag()
    {
        CraftSystem.Get().EndDrag();
    }

}
