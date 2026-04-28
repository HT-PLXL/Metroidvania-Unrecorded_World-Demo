using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    private Player player;
    private Bag_Player bag;
    private UI_SkillSlot[] skillSlots;
    [SerializeField] private RectTransform hpRect;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("快捷物品栏")]
    [SerializeField] private float yOffset = 100;
    [SerializeField] private Transform quickItemSelectParent;
    private UI_QuickItemSelect[] quickItemSelects;
    private UI_QuickItemSlot[] quickItemSlots;
    private UI_QuickItemSlot currentSelectSlot;
    private RectTransform currentRect;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        player.hp.OnHpUpdate += UpdateHealthBar;
        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();
        bag = player.bag;
        bag.OnBagChange += UpdateQuickItemSlotsUI;
        bag.OnQuickSlotUsed += UseButton;
        bag.OnBagChange += UpdateSelectSlot;
    }

    private void UpdateSelectSlot()//更新快捷物品栏选择区
    {
        if (currentSelectSlot == null || currentRect == null)
            return;
        if (quickItemSelectParent.position.y != 6480) //判断快捷物品栏选择界面是否可见
        {
            OpenQuickItemSelect(currentSelectSlot, currentRect);
        }
    }
    public void UseButton(int slotNumber)
    {
        quickItemSlots[slotNumber - 1].SimulateButtonUsed();
    }
    public void UpdateQuickItemSlotsUI()
    {
        Bag_Item[] quickItems = bag.quickItems;
        for (int i = 0; i < quickItems.Length; i++)
            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
    }
    public void OpenQuickItemSelect(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        currentSelectSlot = quickItemSlot;//保存一份
        currentRect = targetRect;

        if (quickItemSelects == null)
            quickItemSelects = quickItemSelectParent.GetComponentsInChildren<UI_QuickItemSelect>(true);
        List<Bag_Item> consumables = bag.itemList.FindAll(item => item.itemData.itemType == ItemType.消耗品);
        if (consumables.Count == 0)
        {
            HideQuickItemSelect();
            return;
        }
        for (int i = 0; i < quickItemSelects.Length; i++)
        {
            if (i < consumables.Count)
            {
                quickItemSelects[i].gameObject.SetActive(true);
                quickItemSelects[i].SetupSelect(quickItemSlot, consumables[i]);
            }
            else
            {
                quickItemSelects[i].gameObject.SetActive(false);
            }
        }

        quickItemSelectParent.position = targetRect.position + Vector3.up * yOffset;
    }
    public void HideQuickItemSelect() => quickItemSelectParent.position = new Vector3(0, 6480);

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);

        // 优先返回已激活的槽
        foreach (UI_SkillSlot slot in skillSlots)
        {
            if (slot.skilltype == skillType && slot.gameObject.activeSelf)
                return slot;
        }

        // 无激活槽时返回隐藏的第一个（不激活）
        foreach (UI_SkillSlot slot in skillSlots)
        {
            if (slot.skilltype == skillType)
                return slot;
        }

        return null;
    }
    private void UpdateHealthBar()
    {
        float currentHp = Mathf.RoundToInt(player.hp.currentHp);//RoundToInt四舍五入取整
        float maxHp = player.stats.GetMaxHp();

        hpText.text = "Hp:" + currentHp + "/" + maxHp;
        hpSlider.value = player.hp.GetHpPercent();
    }
}
