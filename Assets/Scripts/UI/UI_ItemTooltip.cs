using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField][cn("物品名")] private TextMeshProUGUI itemName;
    [SerializeField][cn("物品种类")] private TextMeshProUGUI itemType;
    [SerializeField][cn("物品信息")] private TextMeshProUGUI itemInfo;
    [SerializeField][cn("出售价格")] private TextMeshProUGUI itemPrice;
    [SerializeField][cn("商店提示信息")] private Transform shopItemInfo;

    public void ShowTooltip(bool show, RectTransform targetRect, Bag_Item itemNeedToShow,bool buyPrice = false,bool showShopItemInfo =false)
    {
        base.ShowTooltip(show, targetRect);

        int price = buyPrice ? itemNeedToShow.buyPrice : Mathf.FloorToInt(itemNeedToShow.sellPrice);
        int totalPrice = price * itemNeedToShow.stackCount;

        string fullStackPrice = ($"售价:{price}g - 总价{totalPrice}g");
        string singleStackPrice = ($"售价:{price}g");

        itemPrice.text = itemNeedToShow.stackCount > 1 ? fullStackPrice : singleStackPrice;
        itemName.text = itemNeedToShow.itemData.itemName;
        itemType.text = itemNeedToShow.itemData.itemType.ToString();
        itemInfo.text = itemNeedToShow.GetItemInfo();
    }

}