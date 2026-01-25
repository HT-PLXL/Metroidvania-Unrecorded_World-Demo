using System;
using System.Collections.Generic;
using UnityEngine;
//这个类是背包容器脚本，挂载在player上
public class Bag_Base : MonoBehaviour
{
    public event Action OnBagChange;
    public int maxBagSize = 15;
    public List<Bag_Item> itemList = new List<Bag_Item>();
    protected virtual void Awake()
    {

    }
    public bool CanAddItem() => itemList.Count < maxBagSize;//已有物品格数小于最大格子数
    public bool CanAddStack(Bag_Item itemTryToAddStack)
    {
        List<Bag_Item> stackableItemInBag = itemList.FindAll(item => item.itemData == itemTryToAddStack.itemData);
        foreach(var anyStack in stackableItemInBag)//每个同类可堆叠物品都判断一次是否可继续堆叠
        {
            if(anyStack.CanAddStackCount())
                return true;
        }
        return false;
    }
    public void AddItem(Bag_Item itemNeedAdd)
    {
        Bag_Item item = SearchItem(itemNeedAdd.itemData);//检查背包中是否已有相同物品
        if (item != null && item.CanAddStackCount())//若背包内对应物品不为空且可堆叠
            item.AddStackCount();//增加堆叠数量
        else
            itemList.Add(itemNeedAdd);//新加物品到itemList，占用一个位置
        OnBagChange?.Invoke();//通知UI_bag更新
    }
    public void RemoveItem(Bag_Item itemNeedRemove)
    {
        itemList.Remove(SearchItem(itemNeedRemove.itemData));
        OnBagChange?.Invoke();//通知UI_bag更新
    }
    public Bag_Item SearchItem(ItemDataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);//判断并返回已有物品列表有没有指定的物品
    }

}
