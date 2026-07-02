using TMPro;
using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent itemSlotParent;
    [SerializeField] private UI_EquipSlotParent uiEquipSlotParent;
    [SerializeField] private TextMeshProUGUI gold;
    private Bag_Player bag;
    private UI_CraftPreview craftPreview;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButtons;

    private void Awake()
    {
        SetupCraftListButtons();
    }

    public void SetUpCraftUI(Bag_Storage storage)
    {
        bag = storage.bag;
        bag.OnBagChange -= UpdateUI;
        bag.OnBagChange += UpdateUI;
        UpdateUI();
        craftPreview = GetComponentInChildren<UI_CraftPreview>();
        craftPreview.SetupCraftPreview(storage);
    }
    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();//获取所有槽位
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>();//获取所有木牌

        foreach (var slot in craftSlots)//关闭所有槽位
        {
            slot.gameObject.SetActive(false);
        }

        foreach (var button in craftListButtons)//给所有木牌传同一组槽位，4个木牌就有4个实例
        {
            button.SetCraftSlots(craftSlots);
        }
    }
    private void UpdateUI()
    {
        itemSlotParent.UpdateSlots(bag.itemList);
        uiEquipSlotParent.UpdateEquipmentSlots(bag.equipmentSlotList);
        gold.text = bag.gold.ToString();
    }
}
