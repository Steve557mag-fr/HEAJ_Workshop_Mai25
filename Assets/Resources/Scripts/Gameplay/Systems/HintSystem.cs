using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HintSystem : MonoBehaviour
{
    [SerializeField] GameObject prefabUX;
    [SerializeField] Transform hintContainer;
    [SerializeField] HintObject[] currentHints;
    [SerializeField] HintCraftData[] hintCraftsData;

    public void RefreshHintContainer()
    {
        //Remove all
        for (int i = 0; i < hintContainer.childCount; i++) {
            Destroy(hintContainer.GetChild(i).gameObject);
        }

        //Adding all prefab ux about hintObject
        var inventory = PlayerState.Get().GetFilteredBy<HintObject>();
        foreach(HintObject hintObj in inventory)
        {
            //Instantiate hint UX prefab
            //Add to container
            GameObject go = Instantiate(prefabUX, hintContainer);
            go.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = hintObj.name;

        }
    }

    public void TryCraftHint()
    {
        foreach (HintCraftData data in hintCraftsData) {
            if(currentHints.Equals(data.hints)){
                //do stuff..
                return;
            }
        }
    }

}
