using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ShopSlot : UI_ItemSlot
{
    private Bag_Shop shop;

    public enum ShopSlotType { ShopSlot, PlayerSlot }
    public ShopSlotType slotType;
    public void SetShopUI(Bag_Shop shop)
    {
        this.shop = shop;
    }
    public override void OnPointerDown(PointerEventData eventData)//继承的内置点击检测方法
    {
        if (itemInSlot == null)
            return;

        bool rightButton = eventData.button == PointerEventData.InputButton.Right;
        bool leftButton = eventData.button == PointerEventData.InputButton.Left; //用于设定左键按下时才执行的内容

        if (slotType == ShopSlotType.PlayerSlot)//点击玩家背包栏槽位时
        {
            if (leftButton && itemInSlot.itemData.itemType != ItemType.消耗品)//背包里不会有材料，只需要排除消耗品即可
            {
                //允许玩家装备 装备
                bag.TryEquipItem(itemInSlot);//尝试装备点击的物品
                if (itemInSlot == null)//最后一个有装备的格子在装备后会为空，需要再次判断
                    ui.itemTooltip.ShowTooltip(false, null);
            }
             //base.OnPointerDown(eventData);
            if (rightButton)
            {
                bool SellAll = Input.GetKey(KeyCode.LeftShift);
                shop.TrySellItem(itemInSlot, SellAll);
            }
        }
        else if (slotType == ShopSlotType.ShopSlot && rightButton)//右键点击商店栏槽位时
        {
            bool buyAll = Input.GetKey(KeyCode.LeftShift);
            shop.TryBuyItem(itemInSlot, buyAll);
        }

        ui.itemTooltip.ShowTooltip(false, null);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        if (slotType == ShopSlotType.ShopSlot)
            ui.itemTooltip.ShowTooltip(true, rect, itemInSlot, true, true);//对商品修改为显示购买价格
        else
            ui.itemTooltip.ShowTooltip(true, rect, itemInSlot, false, true);
    }
}
