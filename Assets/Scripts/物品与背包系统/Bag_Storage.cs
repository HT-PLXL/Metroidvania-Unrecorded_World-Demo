using System.Collections.Generic;
using UnityEngine;
//仓库类，包含从父类继承的物品列表itemList（仓库）并声明了附属的材料列表（材料仓库）
public class Bag_Storage : Bag_Base
{
    public Bag_Player bag { get; private set; }
    public List<Bag_Item> materialList;//材料储存列表
    public void SetBag(Bag_Player bag)
    {
        this.bag = bag;//获取bag
    }
    public void FromPlayerToStorage(Bag_Item item, bool AllIn)//AllIn用于判断是否需要全部转移
    {
        if (CanAddItem(item))//看库存能不能添加
        {
            //item里面包含了堆叠数量，直接添加会把堆叠数量也带上
            //所以需要new一个新实例itemNeedAdd，它的数量默认1
            Bag_Item itemNeedAdd = new Bag_Item(item.itemData);

            int Amount = AllIn ? item.stackCount : 1;
            for (int i = 0; i < Amount; i++)
            {
                bag.RemoveOneItem(item);
                AddItem(itemNeedAdd);
            }
        }
        TriggerUpdateUI();
    }
    public void FromStorageToPlayer(Bag_Item item, bool AllIn)
    {
        if (bag.CanAddItem(item))
        {
            Bag_Item itemNeedAdd = new Bag_Item(item.itemData);

            int Amount = AllIn ? item.stackCount : 1;
            for (int i = 0; i < Amount; i++)
            {
                RemoveOneItem(item);
                bag.AddItem(itemNeedAdd);//使用itemNeedAdd原理同上
            }
        }
        TriggerUpdateUI();
    }
    public Bag_Item FindStackableMaterial(Bag_Item itemNeedAdd)//找材料仓库中对应的可堆叠物品
    {
        List<Bag_Item> stackableItems = materialList.FindAll(item => item.itemData == itemNeedAdd.itemData);

        foreach (var item in stackableItems)
        {
            if (item.CanAddStackCount())
            {
                return item;
            }
        }

        return null;
    }
    public void AddMaterialToStorage(Bag_Item itemNeedAdd)//添加进材料仓库里
    {
        var stackableItem = FindStackableMaterial(itemNeedAdd);

        if (stackableItem != null)
            stackableItem.AddStackCount();
        else
            materialList.Add(itemNeedAdd);

        TriggerUpdateUI();
    }
    public int GetAvailableAmount(ItemDataSO requiredItem)//获取物品可用数量
    {
        int amount = 0;

        foreach (var item in bag.itemList)
        {
            if (item.itemData == requiredItem)
                amount = amount + item.stackCount;
        }

        foreach (var item in itemList)
        {
            if (item.itemData == requiredItem)
                amount = amount + item.stackCount;
        }

        foreach (var item in materialList)
        {
            if (item.itemData == requiredItem)
                amount = amount + item.stackCount;
        }

        return amount;
    }
    public bool HasEnoughMaterials(Bag_Item itemWillCraft)//判断是否有足够材料
    {
        foreach (var requiredMaterial in itemWillCraft.itemData.craftRecipe)//配方中的每种材料
        {
            if (GetAvailableAmount(requiredMaterial.itemData) < requiredMaterial.stackCount)//查询可用数量后比较配方的材料的要求数量
                return false;
        }

        return true;
    }
    private int ConsumedMaterialsAmount(List<Bag_Item> itemList, Bag_Item neededItem)//具体消耗方法——给定消耗位置（背包/库存/材料仓库）及消耗对象执行材料消耗
    {
        int amountNeeded = neededItem.stackCount;
        int consumedAmount = 0;

        foreach (var item in itemList)
        {
            if (item.itemData != neededItem.itemData)
                continue;

            int removeAmount = Mathf.Min(item.stackCount, amountNeeded - consumedAmount);
            item.stackCount = item.stackCount - removeAmount;
            consumedAmount = consumedAmount + removeAmount;

            if (item.stackCount <= 0)
                itemList.Remove(item);

            if (consumedAmount >= amountNeeded)
                break;
        }

        return consumedAmount;
    }
    public void ConsumeMaterials(Bag_Item itemWillCraft)//消耗材料
    {
        foreach (var requiredItem in itemWillCraft.itemData.craftRecipe)//制作配方是物品类，里面包含stackCount
        {
            int amountNeedConsume = requiredItem.stackCount;

            amountNeedConsume = amountNeedConsume - ConsumedMaterialsAmount(bag.itemList, requiredItem);//消耗背包材料

            if (amountNeedConsume > 0)
                amountNeedConsume = amountNeedConsume - ConsumedMaterialsAmount(itemList, requiredItem);//消耗库存材料

            if (amountNeedConsume > 0)
                amountNeedConsume = amountNeedConsume - ConsumedMaterialsAmount(materialList, requiredItem);//消耗材料仓库材料
        }
    }
}
