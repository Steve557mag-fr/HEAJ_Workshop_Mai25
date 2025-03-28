using System.ComponentModel.Design;
using UnityEngine;


public class CardBase : ScriptableObject
{

    [SerializeField] protected string cardTitle;
    [SerializeField] protected Sprite image;

    internal virtual void Use() {}

}
