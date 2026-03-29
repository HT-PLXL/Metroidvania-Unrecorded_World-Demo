using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickItemSelect : UI_ItemSlot
{
    public UI_QuickItemSlot currentQuickItemSlot;

    public void SetupSelect(UI_QuickItemSlot currentQuickItemSlot, Bag_Item itemToSet)
    {
        this.currentQuickItemSlot = currentQuickItemSlot;
        UpdateSlot(itemToSet);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        currentQuickItemSlot.SetupQuickSlotItem(itemInSlot);
        ui.gameUI.HideQuickItemSelect();
    }
}
