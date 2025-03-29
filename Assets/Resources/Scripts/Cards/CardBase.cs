using System.ComponentModel.Design;
using UnityEngine;


public class CardBase : ScriptableObject
{

    [SerializeField] public string cardTitle;
    [SerializeField] public Sprite image;

    internal virtual void Use() {}

}
