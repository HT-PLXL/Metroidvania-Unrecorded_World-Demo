using UnityEngine;

public class UI_Shop : MonoBehaviour
{

    private Bag_Player bag;
    private Bag_Shop shop;

    [SerializeField] private UI_ItemSlotParent shopSlotsParent;
    [SerializeField] private UI_ItemSlotParent bagSlotsParent;
    [SerializeField] private UI_EquipSlotParent equipSlotsParent;

    public void SetupShopUI(Bag_Shop shop, Bag_Player bag)
    {
        this.shop = shop;
        this.bag = bag;
        //需要同时给bag和shop事件绑定方法
        //不然使用/增加/移除时bag.OnBagChange或shop.OnBagChange触发，但没有方法，也就不会更新UI
        this.bag.OnBagChange += UpdateSlotUI;
        shop.OnBagChange += UpdateSlotUI;
        UpdateSlotUI();

        UI_ShopSlot[] shopSlots = GetComponentsInChildren<UI_ShopSlot>();
        foreach (var slot in shopSlots)
            slot.SetShopUI(shop);
    }

    private void UpdateSlotUI()
    {
        if (bag == null)
            return;
        bagSlotsParent.UpdateSlots(bag.itemList);
        shopSlotsParent.UpdateSlots(shop.itemList);
        equipSlotsParent.UpdateEquipmentSlots(bag.equipmentSlotList);
    }

}
