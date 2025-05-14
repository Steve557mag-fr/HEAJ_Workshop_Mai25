using System.Collections.Generic;
using UnityEngine;

public interface ICraftSystem
{
    void BeginDrag(int index);
    void Craft();
    void DisplayCraft();
    void EndDrag();
    Transform GetNearestJoint(Transform piece);
    bool IsInRange(GameObject piece, float minX, float maxX, float minY, float maxY);
    bool PatternIsValid(List<Sprite> pieces);
    void Place(Transform piece, Transform at);
    void RefreshCraftList();
    void RefreshGameItemList();
    void RemoveAllParts();
    void SelectCraft(int index);
    void SetUIActive(bool state);
    void SwitchItemDisplay(bool state);
}