using UnityEngine;

[CreateAssetMenu(fileName = "GameItem", menuName = "")]
public class GameItemObject : InventoryItemObject
{
    public bool canBePlaced = true;
    public Sprite itemIcon;
    public Sprite puzzleIcon;
    public GameObject piece;
    public AudioClip soundCue;
}
