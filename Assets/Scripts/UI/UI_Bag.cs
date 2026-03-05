using System.Collections.Generic;
using UnityEngine;
//角色UI的组件，管理子类物品槽
public class UI_Bag : MonoBehaviour
{
    private UI_EquipSlot[] ui_EquipSlots;
    private Bag_Player bag;
    [SerializeField] private UI_ItemSlotParent bagSlotsParent;
    [SerializeField] private Transform uiEquipSlotParent;


    private void Awake()
    {
        ui_EquipSlots = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();
        bag = FindFirstObjectByType<Bag_Player>();
        bag.OnBagChange += UpdateUI;//订阅方法
        UpdateUI();//使初始图标透明，默认状态下背包槽和图标都使用槽位背景图，如果无物品，则对应图标透明
    }
    private void OnEnable()
    {
        if(bag == null)
            return;
        UpdateUI();
    }
    private void UpdateUI()
    {
        bagSlotsParent.UpdateSlots(bag.itemList);
        UpdateEquipmentSlots();
    }

    private void UpdateEquipmentSlots()//更新装备栏
    {
        List<Bag_EquipmentSlot> PlayerEquipList = bag.equipmentSlotList;
        for(int i = 0; i< ui_EquipSlots.Length; i++)
        {
            var PlayerEquipSlot = PlayerEquipList[i];
            if (PlayerEquipSlot.HasItem() == false)
            {
                ui_EquipSlots[i].UpdateSlot(null);
            }
            else
            {
                ui_EquipSlots[i].UpdateSlot(PlayerEquipSlot.equipmentItem);
            }
        }
    }
}
