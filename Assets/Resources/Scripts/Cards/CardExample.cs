using UnityEngine;

public class CardExample : CardBase
{

    [SerializeField] bool iamSteve;

    internal override void Use()
    {
        Debug.Log("salut!! MOOOOOOOOAAAAWWWWW "+ cardTitle);
    }

}
