using System.Collections.Generic;
using UnityEngine;
//此类挂载于具体槽位的父对象，如背包，提供一个更新槽位信息的方法并更新内部所有已有信息的槽位
public class UI_ItemSlotParent : MonoBehaviour
{
    private UI_ItemSlot[] slots;

    public void UpdateSlots(List<Bag_Item> itemList)
    {
        if (slots == null)
            slots = GetComponentsInChildren<UI_ItemSlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)//选择槽位总数列表<已用列表长度的部分，更新内容
            {
                slots[i].UpdateSlot(itemList[i]);
            }
            else//其余部分设置透明
            {
                slots[i].UpdateSlot(null);
            }
        }
    }
}
