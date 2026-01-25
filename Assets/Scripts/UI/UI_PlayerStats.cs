using System.Data;
using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] ui_StatSlots;
    private Bag_Player bag;
    private void Awake()
    {
        ui_StatSlots = GetComponentsInChildren<UI_StatSlot>();
        bag = FindFirstObjectByType<Bag_Player>();
        bag.OnBagChange += UpdateStatsUI;//装备或卸下装备时顺带更新数据
    }
    private void Start()
    {
        UpdateStatsUI();
    }
    private void UpdateStatsUI()
    {
        foreach (var statSlot in ui_StatSlots)
            statSlot.UpdateStatValue();
    }
}
