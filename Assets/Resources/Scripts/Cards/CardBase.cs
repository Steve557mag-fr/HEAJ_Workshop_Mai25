using System.ComponentModel.Design;
using UnityEngine;


public class CardBase : ScriptableObject
{

    [SerializeField] internal string cardTitle;
    [SerializeField] internal Sprite image;

    internal virtual void Use() {}

}
