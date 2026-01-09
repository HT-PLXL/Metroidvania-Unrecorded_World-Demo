using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    private UI_SkillTree skillTree;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [SerializeField] private string meetConditionHex;//满足条件时十六进制数
    [SerializeField] private string NotMeetConditionHex;
    [SerializeField] private string importantInfoHex;

    [SerializeField] private Color color;
    [SerializeField]
    private string lockedSkillText = "神灵降下枷锁\n\n该技能已锁定";

    protected override void Awake()
    {
        base.Awake();
        skillTree = GetComponentInParent<UI_SkillTree>();
    }
    public override void showTooltip(bool show, RectTransform targetRect)
    {
        base.showTooltip(show, targetRect);
    }
    public void showTooltip(bool show, RectTransform targetRect, UI_TreeNode node)//同名函数重载，允许不同参数执行不同内容
    {
        base.showTooltip(show, targetRect);
        if (show == false)
            return;

        skillName.text = node.skillData.skillName;
        skillDescription.text = node.skillData.description;
        string skillLockedText = $"<color={importantInfoHex}>{lockedSkillText}</color>";
        string requirement = node.islocked ? skillLockedText : GetRequirements(node.skillData.cost, node.needNodes, node.conflictNodes);
        skillRequirements.text = requirement;
    }
    private string GetRequirements(int cost, UI_TreeNode[] needNodes, UI_TreeNode[] conflictNodes)//用于得到需求文本
    {
        StringBuilder sb = new StringBuilder();//字符串处理类
        sb.AppendLine("——需 求——");

        string costColor = skillTree.haveEnoughSkillPoint(cost) ? meetConditionHex : NotMeetConditionHex;
        sb.AppendLine($"<color={costColor}>{cost} 技能点</color>");

        foreach (var node in needNodes)
        {
            string nodeColor = node.isUnlocked ? meetConditionHex : NotMeetConditionHex;
            sb.AppendLine($"<color={nodeColor}>{node.skillData.skillName}</color>");
        }
        if (conflictNodes.Length <= 0)
            return sb.ToString();
        sb.AppendLine();//空行
        sb.AppendLine($"<color={importantInfoHex}>-冲突技能-</color>");
        foreach (var node in conflictNodes)
        {
            sb.AppendLine($"<color={importantInfoHex}>{node.skillData.skillName}</color>");
        }
        return sb.ToString();
    }
}
