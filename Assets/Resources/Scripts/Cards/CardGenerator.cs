using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    



    public void DeleteCard(CardBase card)
    {
        Debug.Log("deleted " + card.name + " Card");
    }
}
