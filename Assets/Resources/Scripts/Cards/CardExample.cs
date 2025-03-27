using UnityEngine;

[CreateAssetMenu(fileName = "CardParameters", menuName = "Data/Cards/Card Example")]
public class CardExample : CardBase
{

    [SerializeField] bool iamSteve;

    internal override void Use()
    {
        Debug.Log("salut!! MOOOOOOOOAAAAWWWWW "+ cardTitle);
    }

}
