using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DraggableScriptableObject", menuName = "Scriptable Objects/DraggableScriptableObject")]
public class DraggableScriptableObject : ScriptableObject
{
    
    private void Init()
    {
        //GameObject.transform.Find("ItemSprite").GetComponent<EventTrigger>().onClick.AddListener(() => { BeginDrag(); });
    }

    public void BeginDrag(int i)
    {
        CraftSystem.Get().BeginDrag(i);
    }

    public void EndDrag()
    {
        CraftSystem.Get().EndDrag();
    }

}
