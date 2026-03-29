using System.Collections.Generic;
using System.Linq;
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
        return materialList.Find(item => item.itemData == itemNeedAdd.itemData && item.CanAddStackCount());
    }
    public void AddMaterialToStorage(Bag_Item itemNeedAdd)//添加进材料仓库里
    {
        var stackableItem = FindStackableMaterial(itemNeedAdd);

        if (stackableItem != null)
            stackableItem.AddStackCount();
        else
            materialList.Add(itemNeedAdd);

        TriggerUpdateUI();
        materialList = materialList.OrderBy(item => item.itemData.name).ToList();//名称排序
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
    public override void SavaData(ref GameData data)
    {
        base.SavaData(ref data);

        data.storageItem.Clear();
        foreach (var item in itemList)//itemList是继承的列表（此处是物品仓库）
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.savaId;
                if (data.storageItem.ContainsKey(saveId) == false)//检查字典中是否已有该物品的统计项
                    data.storageItem[saveId] = 0;//如果没有，先初始化堆叠数量为0 确保后续累加不会出错

                data.storageItem[saveId] += item.stackCount;//堆叠数量
            }
        }
        data.storageMaterials.Clear();
        foreach (var item in materialList)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.savaId;
                if (data.storageMaterials.ContainsKey(saveId) == false)//检查字典中是否已有该物品的统计项
                    data.storageMaterials[saveId] = 0;//如果没有，先初始化堆叠数量为0 确保后续累加不会出错

                data.storageMaterials[saveId] += item.stackCount;//堆叠数量
            }
        }
    }
    public override void loadData(GameData data)
    {
        itemList.Clear();//itemList是继承的列表（此处是物品仓库）
        materialList.Clear();//材料仓库
        foreach (var item in data.storageItem)
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
                AddItem(itemNeedLoad);//注意itemList是继承的列表（此处是物品仓库）
            }
        }
        foreach (var item in data.storageMaterials)
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
                AddMaterialToStorage(itemNeedLoad);
            }
        }
    }
}
