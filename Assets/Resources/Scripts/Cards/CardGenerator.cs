using Unity.VisualScripting;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    public string cardName;
    

    public void GenerateCard()
    {

        Debug.Log("GenerateCard Executed");
    }

    public void DeleteCard(CardBase card)
    {
        Debug.Log("deleted " + card.name + " Card");
    }
}
