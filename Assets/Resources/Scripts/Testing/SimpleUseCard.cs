using UnityEngine;

public class SimpleUseCard : MonoBehaviour
{

    [SerializeField] CardBase card;

    void Start()
    {
        card.Use();
    }
}
