using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickItemSlot : UI_ItemSlot
{
    private Button button;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private int slotNumber;

    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }
    public void SimulateButtonUsed()
    {   //EventSystem.current是获取场景中唯一的EventSystem实例,手动将指定的按钮（button.gameObject）设置为Unity事件系统（EventSystem）的当前选中对象。
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        //ExecuteEvents.Execute：手动触发UI事件的方法；new BaseEventData(EventSystem.current)创建一个基础的事件数据（告诉事件系统 “这个事件是谁触发的”）；
        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        //ExecuteEvents.submitHandler指定要触发的事件类型 ——“提交事件”
    }
    public void SetupQuickSlotItem(Bag_Item itemSelect)
    {
        bag.SetQuickItemInSlot(slotNumber, itemSelect);
    }

    public void UpdateQuickSlotUI(Bag_Item currentItemInSlot)
    {
        if (currentItemInSlot == null || currentItemInSlot.itemData == null || bag.itemList.Contains(currentItemInSlot) == false)//第三个条件很重要，防止异常引用
        {
            itemIcon.sprite = defaultSprite;
            itemStackCount.text = "";
            return;
        }

        itemIcon.sprite = currentItemInSlot.itemData.icon;
        itemStackCount.text = currentItemInSlot.stackCount.ToString();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.gameUI.OpenQuickItemSelect(this, rect);
    }
}
