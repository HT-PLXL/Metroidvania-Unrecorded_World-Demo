using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttributeUpgrade : MonoBehaviour
{
    [Header("升级按钮")]
    [SerializeField] private Button upgradeStrengthBtn;
    [SerializeField] private Button upgradeAgilityBtn;
    [SerializeField] private Button upgradeIntelligenceBtn;
    [SerializeField] private Button upgradeVitalityBtn;

    [Header("等级文本")]
    [SerializeField] private TextMeshProUGUI strengthLevelText;
    [SerializeField] private TextMeshProUGUI agilityLevelText;
    [SerializeField] private TextMeshProUGUI intelligenceLevelText;
    [SerializeField] private TextMeshProUGUI vitalityLevelText;

    [Header("属性图标")]
    [SerializeField] private Image strengthIcon;
    [SerializeField] private Image agilityIcon;
    [SerializeField] private Image intelligenceIcon;
    [SerializeField] private Image vitalityIcon;

    [Header("灵魂残响显示")]
    [SerializeField] private TextMeshProUGUI soulEchoText;

    private Player_AttributeManager attributeManager;

    private void Start()
    {
        attributeManager = FindAnyObjectByType<Player_AttributeManager>();
        if (attributeManager != null)
        {
            attributeManager.OnAttributeUpgraded += UpdateUI;
            upgradeStrengthBtn.onClick.AddListener(() => attributeManager.UpgradeAttribute(StatType.Strength));
            upgradeAgilityBtn.onClick.AddListener(() => attributeManager.UpgradeAttribute(StatType.Agility));
            upgradeIntelligenceBtn.onClick.AddListener(() => attributeManager.UpgradeAttribute(StatType.Intelligence));
            upgradeVitalityBtn.onClick.AddListener(() => attributeManager.UpgradeAttribute(StatType.Vitality));
        }
        UpdateUI();
    }
    private void OnEnable()   // 每次激活面板时刷新
    {
        if (attributeManager == null)
            attributeManager = FindAnyObjectByType<Player_AttributeManager>();
        UpdateUI();
    }
    private void OnDestroy()
    {
        if (attributeManager != null)
            attributeManager.OnAttributeUpgraded -= UpdateUI;
    }

    private void UpdateUI()
    {
        if (attributeManager == null) return;

        // 直接从管理器获取灵魂残响数量
        soulEchoText.text = attributeManager.GetAvailableSoulEcho().ToString();

        UpdateButton(upgradeStrengthBtn, strengthLevelText, strengthIcon, attributeManager, StatType.Strength);
        UpdateButton(upgradeAgilityBtn, agilityLevelText, agilityIcon, attributeManager, StatType.Agility);
        UpdateButton(upgradeIntelligenceBtn, intelligenceLevelText, intelligenceIcon, attributeManager, StatType.Intelligence);
        UpdateButton(upgradeVitalityBtn, vitalityLevelText, vitalityIcon, attributeManager, StatType.Vitality);
    }

    private void UpdateButton(Button btn, TextMeshProUGUI levelText, Image icon, Player_AttributeManager manager, StatType type)
    {

        int level = manager.GetLevel(type);
        levelText.text = $"Lv{level}/6";

        bool isMax = manager.IsMaxLevel(type);
        bool canAfford = !isMax && manager.CanUpgrade(type);
        btn.interactable = canAfford;

        string btnText;
        if (isMax)
            btnText = "已满级";
        else
            btnText = $"升级所需：{manager.GetUpgradeCost(type)} 灵魂残响";

        btn.GetComponentInChildren<TextMeshProUGUI>().text = btnText;
        icon.sprite = manager.GetAttributeSprite(type, level);
    }
}