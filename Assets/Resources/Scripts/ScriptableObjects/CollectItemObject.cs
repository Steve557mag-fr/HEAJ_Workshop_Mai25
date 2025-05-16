using UnityEngine;

[CreateAssetMenu(fileName = "Collector", menuName = "Collector Object")]
public class CollectItemObject : ScriptableObject
{

    [SerializeField] InventoryItemObject collectedItem;
    [SerializeField] int quantity = 1;
    [SerializeField] bool markInifite = false;

    public void Run()
    {
        PlayerState.Get().ModifyQuantity(collectedItem, quantity, markInifite);
    }

}
