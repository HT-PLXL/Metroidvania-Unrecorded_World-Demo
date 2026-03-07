using TMPro;
using UnityEngine;
using UnityEngine.UI;
//可选目标项的脚本，绑定button
public class UI_CraftSlot : MonoBehaviour
{
    private ItemDataSO itemWillCraft;
    [SerializeField] private UI_CraftPreview craftPreview;
    [SerializeField] private Image craftItemIcon;
    [SerializeField] private TextMeshProUGUI craftItemName;

    public void SetupButton(ItemDataSO craftData)//为可制作目标设置基础数据
    {
        itemWillCraft = craftData;
        craftItemIcon.sprite = craftData.icon;
        craftItemName.text = craftData.itemName;
    }
    //可选目标的button触发下述更新方法，更新左上详情面版
    public void UpdateCraftPreview() => craftPreview.UpdateCraftPreview(itemWillCraft);
}
