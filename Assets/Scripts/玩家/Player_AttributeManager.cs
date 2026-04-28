using System;
using UnityEngine;

public class Player_AttributeManager : MonoBehaviour, ISaveable
{
    private UI_SkillTree skillTree;

    [Header("属性等级(1~6)-不可为0")]
    [SerializeField] private int strengthLevel = 1;
    [SerializeField] private int agilityLevel = 1;
    [SerializeField] private int intelligenceLevel = 1;
    [SerializeField] private int vitalityLevel = 1;

    private int maxLevel = 6;

    // 每级升级花费（从当前等级升到下一级的花费）
    // 数组长度应等于 maxLevel - 1，索引0 = 等级1→2花费，索引1 = 2→3...索引4 = 5→6
    [Header("升级花费(等级1→2, 2→3...5→6)")]
    [SerializeField] private int[] strengthCosts = { 1, 2, 3, 4, 5 };
    [SerializeField] private int[] agilityCosts = { 1, 2, 3, 4, 5 };
    [SerializeField] private int[] intelligenceCosts = { 1, 2, 3, 4, 5 };
    [SerializeField] private int[] vitalityCosts = { 1, 2, 3, 4, 5 };

    // 各等级对应的基础值（索引0 = 等级1，索引1 = 等级2...索引5 = 等级6）
    [Header("属性基础值(等级1,等级2...等级6)")]
    [SerializeField] private float[] strengthValues = { 2, 4, 6, 8, 10, 12 };
    [SerializeField] private float[] agilityValues = { 2, 4, 6, 8, 10, 12 };
    [SerializeField] private float[] intelligenceValues = { 2, 4, 6, 8, 10, 12 };
    [SerializeField] private float[] vitalityValues = { 2, 4, 6, 8, 10, 12 };

    [Header("属性等级图标(对应等级1~6)")]
    [SerializeField] private Sprite[] strengthSprites;
    [SerializeField] private Sprite[] agilitySprites;
    [SerializeField] private Sprite[] intelligenceSprites;
    [SerializeField] private Sprite[] vitalitySprites;

    public event Action OnAttributeUpgraded;
    private void Start()
    {        
        // 延迟到 Start 确保 UI.Awake 已执行，skillTreeUI 已赋值
        Player player = GetComponent<Player>();
        if (player != null && player.ui != null)
            skillTree = player.ui.skillTreeUI;

        if (skillTree == null)
            skillTree = FindAnyObjectByType<UI_SkillTree>(FindObjectsInactive.Include);

        ApplyAttributeBaseValues();
    }
    // 供外部读取灵魂残响数量（如UI显示）
    public int GetAvailableSoulEcho()
    {
        return skillTree != null ? skillTree.GetSkillPoints() : 0;
    }

    // 扣除灵魂残响 (内部使用)
    private void SpendSoulEcho(int amount)
    {
        if (skillTree != null)
            skillTree.RemoveSkillPoint(amount);
    }
    /// <summary>
    /// 根据配置的等级数组设置四大属性的基础值
    /// </summary>
    private void ApplyAttributeBaseValues()
    {
        Player_Stats stats = GetComponent<Player_Stats>();
        if (stats == null) return;

        // 等级1对应索引0，确保数组长度足够，否则回退到原有公式
        stats.major.strength.SetBaseValue(GetValueForLevel(strengthValues, strengthLevel, 2));
        stats.major.agility.SetBaseValue(GetValueForLevel(agilityValues, agilityLevel, 2));
        stats.major.intelligence.SetBaseValue(GetValueForLevel(intelligenceValues, intelligenceLevel, 2));
        stats.major.vitality.SetBaseValue(GetValueForLevel(vitalityValues, vitalityLevel, 2));
    }
    private float GetValueForLevel(float[] values, int level, float fallbackPerLevel)
    {
        int index = level - 1;
        if (values != null && values.Length > index)
            return values[index];
        // 越界时回退：每级增加 fallbackPerLevel
        return level * fallbackPerLevel;
    }
    public int GetLevel(StatType type)
    {
        return type switch
        {
            StatType.Strength => strengthLevel,
            StatType.Agility => agilityLevel,
            StatType.Intelligence => intelligenceLevel,
            StatType.Vitality => vitalityLevel,
            _ => 1
        };
    }
    public bool IsMaxLevel(StatType type) => GetLevel(type) >= maxLevel;
    /// <summary>
    /// 获取从当前等级升到下一级需要的消耗
    /// </summary>
    public int GetUpgradeCost(StatType type)
    {
        int level = GetLevel(type);
        if (level >= maxLevel) return int.MaxValue; // 已满级
        int[] costs = GetCosts(type);
        int index = level - 1;
        if (costs != null && costs.Length > index)
            return costs[index];
        // 回退：默认按等级递增
        return level;
    }
    private int[] GetCosts(StatType type)
    {
        return type switch
        {
            StatType.Strength => strengthCosts,
            StatType.Agility => agilityCosts,
            StatType.Intelligence => intelligenceCosts,
            StatType.Vitality => vitalityCosts,
            _ => null
        };
    }
    //private float[] GetValues(StatType type)
    //{
    //    return type switch
    //    {
    //        StatType.Strength => strengthValues,
    //        StatType.Agility => agilityValues,
    //        StatType.Intelligence => intelligenceValues,
    //        StatType.Vitality => vitalityValues,
    //        _ => null
    //    };
    //}
    public bool CanUpgrade(StatType type)
    {
        if (IsMaxLevel(type)) return false;
        if (skillTree == null) return false;
        int cost = GetUpgradeCost(type);
        return skillTree.HaveEnoughSkillPoint(cost);
    }
    public void UpgradeAttribute(StatType type)
    {
        if (!CanUpgrade(type)) return;

        int cost = GetUpgradeCost(type);
        SpendSoulEcho(cost);

        Player_Stats stats = GetComponent<Player_Stats>();
        switch (type)
        {
            case StatType.Strength:
                strengthLevel++;
                stats.major.strength.SetBaseValue(GetValueForLevel(strengthValues, strengthLevel, 2));
                break;
            case StatType.Agility:
                agilityLevel++;
                stats.major.agility.SetBaseValue(GetValueForLevel(agilityValues, agilityLevel, 2));
                break;
            case StatType.Intelligence:
                intelligenceLevel++;
                stats.major.intelligence.SetBaseValue(GetValueForLevel(intelligenceValues, intelligenceLevel, 2));
                break;
            case StatType.Vitality:
                vitalityLevel++;
                stats.major.vitality.SetBaseValue(GetValueForLevel(vitalityValues, vitalityLevel, 2));
                break;
        }
        OnAttributeUpgraded?.Invoke();
    }
    public Sprite GetAttributeSprite(StatType type, int level)
    {
        Sprite[] sprites = type switch
        {
            StatType.Strength => strengthSprites,
            StatType.Agility => agilitySprites,
            StatType.Intelligence => intelligenceSprites,
            StatType.Vitality => vitalitySprites,
            _ => null
        };
        int index = level - 1;
        if (sprites != null && index >= 0 && index < sprites.Length)
            return sprites[index];
        return null;
    }
    public void SavaData(ref GameData data)
    {
        data.attributeLevels = new int[] { strengthLevel, agilityLevel, intelligenceLevel, vitalityLevel };
    }
    public void loadData(GameData data)
    {
        if (data.attributeLevels != null && data.attributeLevels.Length == 4)
        {
            strengthLevel = Mathf.Clamp(data.attributeLevels[0], 1, 6);
            agilityLevel = Mathf.Clamp(data.attributeLevels[1], 1, 6);
            intelligenceLevel = Mathf.Clamp(data.attributeLevels[2], 1, 6);
            vitalityLevel = Mathf.Clamp(data.attributeLevels[3], 1, 6);
            ApplyAttributeBaseValues();
        }
    }
}