using UnityEngine;

[CreateAssetMenu(fileName = "ReceiptObject", menuName = "Scriptable Objects/ReceiptObject")]
public class ReceiptObject : ScriptableObject
{

    [SerializeField] CardBase card;
    
}
