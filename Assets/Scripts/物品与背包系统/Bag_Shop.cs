using System.Collections.Generic;
using UnityEngine;
//商店脚本，包含在售物品填充方法
public class Bag_Shop : Bag_Base
{
    private Bag_Player bag;
    [SerializeField] private ItemListDataSO shopData;//商店可进货清单
    [SerializeField][cn("最小商店摊位数")] private int minItemsNumber = 8;//最小在售物品(格)数

    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    }
    public void SetBag(Bag_Player bag)
    {
        this.bag = bag;
    }
    public void TryBuyItem(Bag_Item itemWillBuy, bool buyAll)//商店购买方法
    {
        int amountWillBuy = buyAll ? itemWillBuy.stackCount : 1;
        Bag_Item buy = new Bag_Item(itemWillBuy.itemData);//创建一个堆叠数量为1的新实例

        if (bag.gold < buy.buyPrice * amountWillBuy)//判断钱
        {
            Debug.Log("钱不够!");
            return;
        }
        if (buy.itemData.itemType == ItemType.材料)//判断是不是材料
        {
            for (int i = 0; i < amountWillBuy; i++)
            {
                bag.storage.AddMaterialToStorage(buy);
                RemoveOneItem(itemWillBuy);
            }
            bag.gold = bag.gold - itemWillBuy.buyPrice * amountWillBuy;
        }
        else
        {
            for (int i = 0; i < amountWillBuy; i++)
            {
                if (bag.CanAddItem(buy))
                {
                    bag.AddItem(buy);
                    RemoveOneItem(itemWillBuy);
                    bag.gold = bag.gold - itemWillBuy.buyPrice;
                }
                else
                    break;
            }
        }

        TriggerUpdateUI();
    }
    public void TrySellItem(Bag_Item itemNeedSell, bool sellAll)//商店回收方法
    {
        int amountNeedSell = sellAll ? itemNeedSell.stackCount : 1;

        for (int i = 0; i < amountNeedSell; i++)
        {
            bag.RemoveOneItem(itemNeedSell);
        }
        int sellPrice = Mathf.FloorToInt(itemNeedSell.sellPrice * amountNeedSell);//Mathf.FloorToInt向下取整，对正数直接去掉小数即可，对负数注意要向下取一个整数
        bag.gold = bag.gold + sellPrice;

        TriggerUpdateUI();
    }
    public void FillShopList()//填充在售物品列表
    {
        itemList.Clear();//从父类继承的列表itemList，这里是在售物品清单
        List<Bag_Item> possibleItems = new List<Bag_Item>();//已选进货清单

        foreach (var itemData in shopData.itemList)//shopData.itemList是物品数据数组，这里是 商店可进货清单
        {
            //randomziedStack:随机堆叠数量
            int randomziedStack = Random.Range(itemData.minStackCountInShop, itemData.maxStackCountInShop + 1);//对int类型[左闭右开)，对float类型[闭区间]，此处包含最大值需要+1
            int finalStack = Mathf.Clamp(randomziedStack, 1, itemData.maxStackCount);
            //用在售清单数据创建物品实例
            Bag_Item itemToAdd = new Bag_Item(itemData);
            itemToAdd.stackCount = finalStack;
            //把物品实例加进 已选进货清单
            possibleItems.Add(itemToAdd);
        }
        //计算随机在售格子数
        int randomziedItemAmount = Random.Range(minItemsNumber, maxBagSize + 1);//maxBagSize最大商店尺寸（格子数）
        int finalAmount = Mathf.Clamp(randomziedItemAmount, 1, possibleItems.Count);
        //根据随机的在售格子数尝试随机进货
        for (int i = 0; i < 8; i++)
        {
            //选择 已选进货清单的随机物品，尝试加入在售清单，成功加入则在已选进货清单中去除
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (CanAddItem(item))
            {
                possibleItems.Remove(item);
                itemList.Add(item);//直接添加进新的摊位

                //AddItem(item);//物品添加在处理带堆叠的物品时存在漏洞
            }
        }

        TriggerUpdateUI();//触发各种库存UI更新
    }
}
