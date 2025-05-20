using Articy.Test;
using Articy.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Collector", menuName = "Collector Object")]
public class CollectItemObject : ScriptableObject
{
    [SerializeField] InventoryItemObject collectedItem;
    //[SerializeField] int quantity = 1;
    //[SerializeField] bool markInifite = false;

    [ArticyTypeConstraint(typeof(Hub))]
    [SerializeField] ArticyRef itemNode;

    static ClickableInteraction currentRef;

    public void Run(ClickableInteraction self)
    {
        currentRef = self;
        Debug.Log(collectedItem.name);
        NarrationSystem.Get().StartWithHub(itemNode.GetObject<Hub>(), collectedItem.name);
    }

    public static void TriggerEndCollect()
    {
        if (currentRef == null) return;
        Destroy(currentRef.gameObject);

    }

}
