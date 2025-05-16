using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ModalSystem : MonoBehaviour
{
    [SerializeField] GameObject newItemUI;
    [SerializeField] GameObject gamePlayUI;
    [SerializeField] Image image;

    public void OpenUI(GameItemObject item)
    {
        newItemUI.SetActive(true);
        gamePlayUI.SetActive(false);
        UpdateItemDisplay(item);
    }

    public void UpdateItemDisplay(GameItemObject item)
    {
        image.sprite = item.itemIcon;
        newItemUI.GetComponentInChildren<TextMeshProUGUI>().text = $"You Got {item.name}";
    }

    public void CloseUI()
    {
        newItemUI.SetActive(false);
        gamePlayUI.SetActive(true);
    }
}
