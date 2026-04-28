using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] ui_StatSlots;
    private Bag_Player bag;
    private Player_AttributeManager attributeManager;
    private void Awake()
    {
        ui_StatSlots = GetComponentsInChildren<UI_StatSlot>();
        bag = FindFirstObjectByType<Bag_Player>();
        attributeManager = FindAnyObjectByType<Player_AttributeManager>();

        if (bag != null)
            bag.OnBagChange += UpdateStatsUI;
        if (attributeManager != null)
            attributeManager.OnAttributeUpgraded += UpdateStatsUI;
    }
    private void Start()
    {
        UpdateStatsUI();
    }
    private void OnEnable()
    {
        UpdateStatsUI();// 面板显示时立即刷新
    }
    private void OnDestroy()
    {
        if (bag != null)
            bag.OnBagChange -= UpdateStatsUI;
        if (attributeManager != null)
            attributeManager.OnAttributeUpgraded -= UpdateStatsUI;
    }
    private void UpdateStatsUI()
    {
        foreach (var statSlot in ui_StatSlots)
            statSlot.UpdateStatValue();
    }
}
