using System.Collections.Generic;
using UnityEngine;
//角色UI的组件，管理子类物品槽
public class UI_Bag : MonoBehaviour
{
    private UI_ItemSolt[] itemSlots;
    private Bag_Base bag;

    private void Awake()
    {
        itemSlots = GetComponentsInChildren<UI_ItemSolt>();
        bag = FindFirstObjectByType<Bag_Base>();
        bag.OnBagChange += UpdateBagSolts;//订阅方法
        UpdateBagSolts();//使初始图标透明，默认状态下背包槽和图标都使用槽位背景图，如果无物品，则对应图标透明
    }
    private void UpdateBagSolts()//更新背包库存
    {
        List<Bag_Item> itemList = bag.itemList;//右为已用槽位列表
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < itemList.Count)//选择槽位总数列表<已用列表长度的部分，更新内容
            {
                itemSlots[i].UpdateSlot(itemList[i]);
            }
            else//其余部分设置透明
            {
                itemSlots[i].UpdateSlot(null);
            }
        }
    }
}
