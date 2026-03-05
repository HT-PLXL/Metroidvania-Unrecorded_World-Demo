using System.Collections.Generic;

public class Bag_Storage : Bag_Base
{
    private Bag_Player bag;
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

            int Amount = AllIn ? item.StackCount : 1;
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

            int Amount = AllIn ? item.StackCount : 1;
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
}
