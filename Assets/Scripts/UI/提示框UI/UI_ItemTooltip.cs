using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField][cn("物品名")] private TextMeshProUGUI itemName;
    [SerializeField][cn("物品种类")] private TextMeshProUGUI itemType;
    [SerializeField][cn("物品信息")] private TextMeshProUGUI itemInfo;
    [SerializeField][cn("出售价格")] private TextMeshProUGUI itemPrice;
    [SerializeField][cn("商店提示信息")] private Transform shopItemInfo;
    [SerializeField][cn("删除提示信息")] private Transform deleteItemInfo;

    public void ShowTooltip(bool show, RectTransform targetRect, Bag_Item itemNeedToShow, bool buyPrice = false, bool showShopItemInfo = false)
    {
        base.ShowTooltip(show, targetRect);

        shopItemInfo.gameObject.SetActive(showShopItemInfo);
        deleteItemInfo.gameObject.SetActive(!showShopItemInfo);

        int price = buyPrice ? itemNeedToShow.buyPrice : Mathf.FloorToInt(itemNeedToShow.sellPrice);
        int totalPrice = price * itemNeedToShow.stackCount;

        string fullStackPrice = ($"售价:{price}g - 总价{totalPrice}g");
        string singleStackPrice = ($"售价:{price}g");

        itemPrice.text = itemNeedToShow.stackCount > 1 ? fullStackPrice : singleStackPrice;
        itemType.text = itemNeedToShow.itemData.itemType.ToString();
        itemInfo.text = itemNeedToShow.GetItemInfo();
        string color = GetColorByRarity(itemNeedToShow.itemData.itemRarity);
        itemName.text = GetColorText(color, itemNeedToShow.itemData.itemName);
    }
    public string GetColorByRarity(int rarity)//确定物品颜色（品质）
    {
        if (rarity < 341)//对应 66.1% - 100% 掉落率
            return "white";//白色
        if (rarity < 501)//对应 50.1% - 66% 掉落率
            return "#32CD32";//绿色
        if (rarity < 751)//对应 25.1% - 50% 掉落率
            return "#1E90FF";//蓝色
        if (rarity < 851)//对应 15.1% - 25% 掉落率
            return "#9932CC";//紫色
        if (rarity < 951)//对应 5.1% - 15% 掉落率
            return "#FFD700";//金色
        return "red";    //对应 0.1% - 5% 掉落率

    }
}