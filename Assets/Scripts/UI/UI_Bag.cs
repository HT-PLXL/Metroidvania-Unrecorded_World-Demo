using System.Collections.Generic;
using UnityEngine;
//角色UI的组件，管理子类物品槽
public class UI_Bag : MonoBehaviour
{
    private UI_ItemSolt[] ui_ItemSlots;
    private UI_EquipSlot[] ui_EquipSlots;
    private Bag_Player bag;
    [SerializeField] private Transform uiItemSlotParent;//UI_EquipSlot继承UI_ItemSolt，Awake直接获取子组件会获得父子组件的混合列表，因此序列化后在unity面板选择并在此获取具体实例的子组件
    [SerializeField] private Transform uiEquipSlotParent;


    private void Awake()
    {
        ui_ItemSlots = uiItemSlotParent.GetComponentsInChildren<UI_ItemSolt>();
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
        UpdateBagSolts();
        UpdateEquipmentSlots();
    }
    private void UpdateBagSolts()//更新背包库存
    {
        List<Bag_Item> itemList = bag.itemList;//右为已用槽位列表
        for (int i = 0; i < ui_ItemSlots.Length; i++)
        {
            if (i < itemList.Count)//选择槽位总数列表<已用列表长度的部分，更新内容
            {
                ui_ItemSlots[i].UpdateSlot(itemList[i]);
            }
            else//其余部分设置透明
            {
                ui_ItemSlots[i].UpdateSlot(null);
            }
        }
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
