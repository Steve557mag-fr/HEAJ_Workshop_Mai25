using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ModalSystem : MonoBehaviour
{
    [SerializeField] Canvas newItemUI;
    [SerializeField] Canvas gamePlayUI;
    [SerializeField] Image image;

    public void OpenUI(GameItemObject item)
    {
        newItemUI.gameObject.SetActive(true);
        gamePlayUI.gameObject.SetActive(false);
        UpdateItemDisplay(item);
    }

    public void UpdateItemDisplay(GameItemObject item)
    {
        image.sprite = item.itemIcon;
        newItemUI.GetComponentInChildren<TextMeshProUGUI>().text = new string($"You Got {item.name}");
    }

    public void CloseUI()
    {
        newItemUI.gameObject.SetActive(false);
        gamePlayUI.gameObject.SetActive(true);
    }
}
