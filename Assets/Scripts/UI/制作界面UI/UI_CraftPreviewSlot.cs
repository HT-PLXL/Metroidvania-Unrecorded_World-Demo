using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviewSlot : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialRequired;

    public void SetupRecipeSlot(ItemDataSO itemData, int avaliableAmount, int requiredAmount)//更新配方槽UI，物品数据-配方，可用数量，需求数量
    {
        materialIcon.sprite = itemData.icon;
        materialRequired.text = itemData.itemName + " - " + avaliableAmount + "/" + requiredAmount;
    }
}
