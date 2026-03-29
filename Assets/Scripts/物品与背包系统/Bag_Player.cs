using System;
using System.Collections.Generic;
using UnityEngine;

public class Bag_Player : Bag_Base
{
    public event Action<int> OnQuickSlotUsed;//有关模拟按钮点击事件
    public int gold = 10000;
    public List<Bag_EquipmentSlot> equipmentSlotList;//玩家装备槽实例，由unity面板添加
    public Bag_Storage storage { get; private set; }
    [Header("快捷物品槽")]
    public Bag_Item[] quickItems = new Bag_Item[2];

    protected override void Awake()
    {
        base.Awake();
        storage = FindFirstObjectByType<Bag_Storage>();
    }
    public void SetQuickItemInSlot(int slotNumber, Bag_Item itemNeedSet)
    {
        quickItems[slotNumber - 1] = itemNeedSet;
        TriggerUpdateUI();
    }
    public void TryUseQuickItemInSlot(int slotNumber)
    {
        Bag_Item itemNeedUse = quickItems[slotNumber - 1];
        if (itemNeedUse == null)
            return;
        TryUseItem(itemNeedUse);
        if (SearchItem(itemNeedUse) == null)//快捷物品槽的本组物品用完
            quickItems[slotNumber - 1] = SearchSameItem(itemNeedUse);//找同类

        TriggerUpdateUI();

        OnQuickSlotUsed?.Invoke(slotNumber);
    }
    public void TryEquipItem(Bag_Item clickedItem)
    {
        Bag_Item item = SearchItem(clickedItem);
        List<Bag_EquipmentSlot> slots = equipmentSlotList.FindAll(slot => slot.slotType == item.itemData.itemType);//判断并找出所有装备槽列表里与想要装备的物品相同种类的
        foreach (var slot in slots)//遍历所有同类装备槽
        {
            if (slot.HasItem() == false)//判断是否有空槽位可用
            {
                EquipItem(item, slot);
                return;
            }
        }
        //装备替换功能，替换第一个槽位物品
        //————————————————————————
        var slotNeedReplace = slots[0];
        var itemNeedRemove = slotNeedReplace.equipmentItem;
        RemoveEquipItem(itemNeedRemove, slotNeedReplace != null);//当槽位满了或背包满了才会需要替换功能，slotNeedReplace !=null将会为true
        EquipItem(item, slotNeedReplace);
    }
    private void EquipItem(Bag_Item itemNeedEquip, Bag_EquipmentSlot slot)
    {
        slot.equipmentItem = itemNeedEquip;
        slot.equipmentItem.AddModifiers(player.stats);
        slot.equipmentItem.AddItemEffect(player);//装备新装备时尝试订阅物品效果
        RemoveOneItem(itemNeedEquip);//从已有物品列表移除刚刚装备的物品
    }
    //当有相同被动效果的装备时，装备后再卸下，一次性清除了所有同效果装备对player实例的引用，导致其他同效果装备引用异常报错，所以需要先卸后装
    //而背包满了先卸后装会导致下方方法的if语句执行，从而吞掉卸下的装备，所以需要加上新的bool值判断跳过判断
    public void RemoveEquipItem(Bag_Item itemNeedToRemove, bool NeedReplace = false)
    {
        if (CanAddItem(itemNeedToRemove) == false && NeedReplace == false)//NeedReplace为true时说明在替换装备，跳过此处的if语句，防止吞装备
        {
            Debug.Log("没有空间去卸下装备");
            return;
        }
        foreach (var slot in equipmentSlotList)
        {
            if (slot.equipmentItem == itemNeedToRemove)
            {
                slot.equipmentItem = null;
                break;
            }
        }
        itemNeedToRemove.RemoveModifiers(player.stats);
        itemNeedToRemove.RemoveItemEffect();//卸下新装备时尝试取消订阅物品效果
        AddItem(itemNeedToRemove);//放回物品栏
    }


    public override void SavaData(ref GameData data)//修改data即在修改游戏存档数据
    {
        data.gold = gold;
        data.bag.Clear();//清空背包
        data.AlreadyEquipedItems.Clear();//清空装备槽
        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.savaId;
                if (data.bag.ContainsKey(saveId) == false)//检查字典中是否已有该物品的统计项
                    data.bag[saveId] = 0;//如果没有，先初始化堆叠数量为0 确保后续累加不会出错

                data.bag[saveId] += item.stackCount;//堆叠数量
            }
        }
        foreach (var slot in equipmentSlotList)
        {
            if (slot.HasItem())
                data.AlreadyEquipedItems[slot.equipmentItem.itemData.savaId] = slot.slotType;
        }
    }
    public override void loadData(GameData data)
    {
        gold = data.gold;
        foreach (var item in data.bag)
        {
            string saveId = item.Key;
            int stackCount = item.Value;
            //注意及时更新 所有物品列表 数据容器
            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            if (itemData == null)
            {
                Debug.LogWarning("物品未找到" + saveId);
                continue;
            }

            for (int i = 0; i < stackCount; i++)
            {
                Bag_Item itemNeedLoad = new Bag_Item(itemData);
                AddItem(itemNeedLoad);
            }
        }
        foreach (var item in data.AlreadyEquipedItems)
        {

            string saveId = item.Key;
            ItemType loadedSlotType = item.Value;
            ItemDataSO itemData = itemDataBase.GetItemData(saveId);
            Bag_Item itemNeedLoad = new Bag_Item(itemData);//用数据创建新物品

            var slot = equipmentSlotList.Find(slot => slot.slotType == loadedSlotType && slot.HasItem() == false);
            slot.equipmentItem = itemNeedLoad;
            slot.equipmentItem.AddModifiers(player.stats);
            slot.equipmentItem.AddItemEffect(player);
        }
        TriggerUpdateUI();//订阅更新角色数据表方法
    }
}
