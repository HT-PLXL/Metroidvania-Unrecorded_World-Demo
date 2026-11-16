using System;
using System.Collections.Generic;
using UnityEngine;

public class Bag_Player : Bag_Base
{
    public event Action<int> OnQuickSlotUsed;
    public int gold = 10000;
    public List<Bag_EquipmentSlot> equipmentSlotList;
    public Bag_Storage storage { get; private set; }
    [Header("快捷物品槽")]
    public Bag_Item[] quickItems = new Bag_Item[2];

    protected override void Awake()
    {
        base.Awake();
        storage = FindFirstObjectByType<Bag_Storage>();
    }
    public void AddGold(int needAddOrReduce)
    {
        gold += needAddOrReduce;
    }
    public void SetQuickItemInSlot(int slotNumber, Bag_Item itemNeedSet)
    {
        quickItems[slotNumber - 1] = itemNeedSet;
        TriggerUpdateUI();
    }

    public void TryUseQuickItemInSlot(int slotNumber)
    {
        Bag_Item itemNeedUse = quickItems[slotNumber - 1];
        if (itemNeedUse == null) return;
        TryUseItem(itemNeedUse);
        if (SearchItem(itemNeedUse) == null)
            quickItems[slotNumber - 1] = SearchSameItem(itemNeedUse);
        TriggerUpdateUI();
        OnQuickSlotUsed?.Invoke(slotNumber);
    }

    public void TryEquipItem(Bag_Item clickedItem)
    {
        Bag_Item item = SearchItem(clickedItem);
        List<Bag_EquipmentSlot> slots = equipmentSlotList.FindAll(slot => slot.slotType == item.itemData.itemType);
        foreach (var slot in slots)
        {
            if (!slot.HasItem())
            {
                EquipItem(item, slot);
                return;
            }
        }
        var slotNeedReplace = slots[0];
        RemoveEquipItem(slotNeedReplace.equipmentItem, true);
        EquipItem(item, slotNeedReplace);
    }

    private void EquipItem(Bag_Item itemNeedEquip, Bag_EquipmentSlot slot)
    {
        slot.equipmentItem = itemNeedEquip;
        itemNeedEquip.AddModifiers(player.stats);
        itemNeedEquip.AddItemEffect(player);

        EquipmentDataSO equipData = itemNeedEquip.itemData as EquipmentDataSO;
        if (equipData != null)
        {
            player.stats.immunePhysical |= equipData.immunePhysical;
            player.stats.immuneFire |= equipData.immuneFire;
            player.stats.immuneIce |= equipData.immuneIce;
            player.stats.immuneLightning |= equipData.immuneLightning;
            if (equipData.weaponElement != ElementType.None)
                player.stats.SetWeaponElement(equipData.weaponElement);
        }
        RemoveOneItem(itemNeedEquip);
    }

    public void RemoveEquipItem(Bag_Item itemNeedToRemove, bool NeedReplace = false)
    {
        if (!CanAddItem(itemNeedToRemove) && !NeedReplace)
        {
            Debug.Log("没有空间去卸下装备");
            return;
        }
        foreach (var slot in equipmentSlotList)
            if (slot.equipmentItem == itemNeedToRemove) { slot.equipmentItem = null; break; }
        itemNeedToRemove.RemoveModifiers(player.stats);
        itemNeedToRemove.RemoveItemEffect();

        EquipmentDataSO equipData = itemNeedToRemove.itemData as EquipmentDataSO;
        if (equipData != null)
        {
            player.stats.immunePhysical &= !equipData.immunePhysical;
            player.stats.immuneFire &= !equipData.immuneFire;
            player.stats.immuneIce &= !equipData.immuneIce;
            player.stats.immuneLightning &= !equipData.immuneLightning;

            if (equipData.weaponElement != ElementType.None && player.stats.currentWeaponElement == equipData.weaponElement)
            {
                bool found = false;
                foreach (var slot in equipmentSlotList)
                {
                    if (slot.HasItem())
                    {
                        EquipmentDataSO otherEquip = slot.equipmentItem.itemData as EquipmentDataSO;
                        if (otherEquip != null && otherEquip.weaponElement != ElementType.None)
                        {
                            player.stats.SetWeaponElement(otherEquip.weaponElement);
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) player.stats.SetWeaponElement(ElementType.None);
            }
        }
        AddItem(itemNeedToRemove);
    }

    public override void SavaData(ref GameData data)
    {
        data.gold = gold;
        data.bag.Clear();
        foreach (var item in itemList)
        {
            if (item?.itemData != null)
            {
                string saveId = item.itemData.savaId;
                if (string.IsNullOrEmpty(saveId))
                {
                    Debug.LogWarning($"物品 {item.itemData.name} 的 savaId 为空，跳过保存");
                    continue;
                }
                if (!data.bag.ContainsKey(saveId)) data.bag[saveId] = 0;
                data.bag[saveId] += item.stackCount;
            }
        }

        data.AlreadyEquipedItems.Clear();
        foreach (var slot in equipmentSlotList)
        {
            if (slot.HasItem())
                data.AlreadyEquipedItems[slot.equipmentItem.itemData.savaId] = slot.slotType;
        }

        //data.quickItemSaveIds = new string[2];
        //for (int i = 0; i < quickItems.Length; i++)
        //    data.quickItemSaveIds[i] = quickItems[i]?.itemData.savaId;
        data.quickItemSaveIds = new string[2];
        for (int i = 0; i < quickItems.Length; i++)
        {
            // 确保 Bag_Item 和它的 itemData 都有效
            if (quickItems[i] != null && quickItems[i].itemData != null)
                data.quickItemSaveIds[i] = quickItems[i].itemData.savaId;
            else
                data.quickItemSaveIds[i] = null;   // 空槽或无效物品
        }
    }

    public override void loadData(GameData data)
    {
        if (player == null) player = GetComponent<Player>();
        if (player.stats == null) player.stats = player.GetComponent<Player_Stats>();

        gold = data.gold;
        itemList.Clear();

        // 加载背包
        if (data.bag != null)
            foreach (var kv in data.bag)
            {
                ItemDataSO itemData = itemDataBase?.GetItemData(kv.Key);
                if (itemData == null) continue;
                for (int i = 0; i < kv.Value; i++)
                    AddItem(new Bag_Item(itemData));
            }

        // 清空并重置装备槽
        foreach (var slot in equipmentSlotList)
        {
            if (slot.HasItem())
            {
                slot.equipmentItem.RemoveModifiers(player.stats);
                slot.equipmentItem.RemoveItemEffect();
                slot.equipmentItem = null;
            }
        }
        player.stats.immunePhysical = false;
        player.stats.immuneFire = false;
        player.stats.immuneIce = false;
        player.stats.immuneLightning = false;
        player.stats.SetWeaponElement(ElementType.None);

        // 加载装备
        if (data.AlreadyEquipedItems != null)
            foreach (var kv in data.AlreadyEquipedItems)
            {
                ItemDataSO itemData = itemDataBase?.GetItemData(kv.Key);
                if (itemData == null) continue;
                Bag_Item newItem = new Bag_Item(itemData);
                var slot = equipmentSlotList.Find(s => s.slotType == kv.Value && !s.HasItem());
                if (slot != null)
                {
                    slot.equipmentItem = newItem;
                    newItem.AddModifiers(player.stats);
                    newItem.AddItemEffect(player);
                    EquipmentDataSO equip = itemData as EquipmentDataSO;
                    if (equip != null)
                    {
                        player.stats.immunePhysical |= equip.immunePhysical;
                        player.stats.immuneFire |= equip.immuneFire;
                        player.stats.immuneIce |= equip.immuneIce;
                        player.stats.immuneLightning |= equip.immuneLightning;
                        if (equip.weaponElement != ElementType.None)
                            player.stats.SetWeaponElement(equip.weaponElement);
                    }
                }
            }

        // 加载快捷槽
        //if (data.quickItemSaveIds != null)
        //    for (int i = 0; i < quickItems.Length && i < data.quickItemSaveIds.Length; i++)
        //        quickItems[i] = itemList.Find(item => item.itemData.savaId == data.quickItemSaveIds[i]);

        // 加载快捷槽
        if (data.quickItemSaveIds != null)
        {
            for (int i = 0; i < quickItems.Length && i < data.quickItemSaveIds.Length; i++)
            {
                string id = data.quickItemSaveIds[i];
                if (!string.IsNullOrEmpty(id))
                {
                    //确保 itemData 存在
                    ItemDataSO itemData = itemDataBase?.GetItemData(id);
                    if (itemData != null)
                        quickItems[i] = itemList.Find(item => item.itemData == itemData);
                    else
                        quickItems[i] = null;
                }
                else
                    quickItems[i] = null;
            }
        }

        TriggerUpdateUI();
    }
}