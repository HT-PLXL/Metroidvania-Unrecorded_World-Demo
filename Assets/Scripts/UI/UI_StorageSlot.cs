using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private Bag_Storage storage;
    //定义一个库存槽位种类的枚举
    public enum StorageSlotType { StorageSlot,PlayerBagSlot}
    public StorageSlotType slotType;

    public void SetStorage(Bag_Storage storage)
    {
        this.storage = storage;
    }
    public override void OnPointerDown(PointerEventData eventData)//继承父类的接口赋予的方法，确定玩家点击背包的物品槽实例
    {
        if (itemInSlot == null)
            return;

        bool AllIn = Input.GetKey(KeyCode.LeftShift);//旧输入系统
        if (slotType == StorageSlotType.StorageSlot)
            storage.FromStorageToPlayer(itemInSlot, AllIn);

        if (slotType == StorageSlotType.PlayerBagSlot)
            storage.FromPlayerToStorage(itemInSlot, AllIn);

        ui.itemToolTip.ShowTooltip(false, null);
    }
}
