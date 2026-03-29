using System.Collections.Generic;
using TMPro;
using UnityEngine;
//角色UI的组件，管理子类物品槽
public class UI_Bag : MonoBehaviour
{
    private Bag_Player bag;
    [SerializeField] private UI_ItemSlotParent bagSlotsParent;
    [SerializeField] private UI_EquipSlotParent uiEquipSlotParent;
    [SerializeField] private TextMeshProUGUI gold;

    private void Awake()
    {
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
        uiEquipSlotParent.UpdateEquipmentSlots(bag.equipmentSlotList);
        gold.text = bag.gold.ToString();
    }


}
