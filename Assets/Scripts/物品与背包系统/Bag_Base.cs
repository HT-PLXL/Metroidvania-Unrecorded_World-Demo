using System;
using System.Collections.Generic;
using UnityEngine;
//这个类是背包容器脚本基类
public class Bag_Base : MonoBehaviour, ISaveable
{
    public event Action OnBagChange;
    protected Player player;
    public int maxBagSize = 15;
    public List<Bag_Item> itemList = new List<Bag_Item>();//注意itemList根据子类变化
    [SerializeField] protected ItemListDataSO itemDataBase;
    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }
    public void TryUseItem(Bag_Item itemNeedUse)//使用消耗品
    {
        Bag_Item consumable = itemList.Find(item => item == itemNeedUse);
        if (consumable == null)
            return;
        if (consumable.itemEffect.CanBeUsed(player) == false)//针对buff消耗品
            return;
        consumable.itemEffect.ExecuteEffect();//执行道具效果
        if (consumable.stackCount > 1)
            consumable.ReduceStackCount();//减少数量
        else
            RemoveOneItem(consumable);//数量为1直接移除
        OnBagChange?.Invoke();//更新UI
    }
    public bool CanAddItem(Bag_Item itemNeedToAdd)
    {
        bool hasStackable = SearchStackableItem(itemNeedToAdd) != null;
        return hasStackable || itemList.Count < maxBagSize;//已有物品格数小于最大格子数
    }

    public Bag_Item SearchStackableItem(Bag_Item itemTryToAddStack)
    {
        return itemList.Find(item => item.itemData == itemTryToAddStack.itemData && item.CanAddStackCount());
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
    public void RemoveOneItem(Bag_Item itemNeedRemove)
    {
        Bag_Item item = itemList.Find(item => item == itemNeedRemove);
        if (item.stackCount > 1)
            item.ReduceStackCount();
        else
            itemList.Remove(itemNeedRemove);
        OnBagChange?.Invoke();//通知UI_bag更新
    }
    public Bag_Item SearchItem(Bag_Item itemNeedFind)//注意找的是指定的单组物品，和下面方法不同
    {
        return itemList.Find(item => item == itemNeedFind);//判断并返回已有物品列表有没有指定的物品
    }
    public Bag_Item SearchSameItem(Bag_Item itemNeedFind)//注意找的是物品数据，和上面方法不同
    {
        return itemList.Find(item => item.itemData == itemNeedFind.itemData);//判断并返回已有物品列表有没有指定的物品
    }
    public void TriggerUpdateUI() => OnBagChange?.Invoke();//触发所有绑定了库存变动事件OnBagChange的UI更新方法
    public virtual void SavaData(ref GameData data)
    {
        //在子类重写
    }
    public virtual void loadData(GameData data)
    {
        //在子类重写
    }

}
