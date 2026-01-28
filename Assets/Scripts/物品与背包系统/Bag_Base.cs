using System;
using System.Collections.Generic;
using UnityEngine;
//这个类是背包容器脚本，挂载在player上
public class Bag_Base : MonoBehaviour, ISaveable
{
    public event Action OnBagChange;
    public int maxBagSize = 15;
    public List<Bag_Item> itemList = new List<Bag_Item>();
    protected virtual void Awake()
    {

    }
    public void TryUseItem(Bag_Item itemNeedUse)
    {
        Bag_Item consumable = itemList.Find(item => item == itemNeedUse);
        if (consumable == null)
            return;
        consumable.itemEffect.ExecuteEffect();//执行道具效果
        if (consumable.StackCount > 1)
            consumable.ReduceStackCount();//减少数量
        else
            RemoveItem(consumable);//数量为1直接移除
        OnBagChange?.Invoke();//更新UI
    }
    public bool CanAddItem() => itemList.Count < maxBagSize;//已有物品格数小于最大格子数
    public Bag_Item SearchStackableItem(Bag_Item itemTryToAddStack)
    {
        List<Bag_Item> stackableItemInBag = itemList.FindAll(item => item.itemData == itemTryToAddStack.itemData);
        foreach (var anyStackableItem in stackableItemInBag)//每个同类可堆叠物品都判断一次是否可继续堆叠
        {
            if (anyStackableItem.CanAddStackCount())
                return anyStackableItem;//找到任意可继续堆叠的物品就结束
        }
        return null;
    }
    public void AddItem(Bag_Item itemNeedAdd)
    {
        Bag_Item item = SearchStackableItem(itemNeedAdd);
        if (item != null)
            item.AddStackCount();//增加堆叠数量
        else
            itemList.Add(itemNeedAdd);//新加物品到itemList，占用一个位置
        OnBagChange?.Invoke();//通知UI_bag更新
    }
    public void RemoveItem(Bag_Item itemNeedRemove)
    {
        itemList.Remove(itemNeedRemove);
        OnBagChange?.Invoke();//通知UI_bag更新
    }
    public Bag_Item SearchItem(ItemDataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);//判断并返回已有物品列表有没有指定的物品
    }
    public void TriggerUpdateUI()=>OnBagChange?.Invoke();//订阅更新角色数据表方法
    public virtual void loadData(GameData data)
    {

    }

    public virtual void SavaData(ref GameData data)
    {

    }
}
