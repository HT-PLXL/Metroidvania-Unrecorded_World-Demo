using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    private UI ui;
    private UI_SkillTree skillTree;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [SerializeField] private string meetConditionHex;//满足条件时十六进制数
    [SerializeField] private string NotMeetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private string WarningHex;

    [SerializeField] private Color testColor;
    [SerializeField] private string lockedSkillText = "此技能路线已被锁定";
    public Coroutine textEffectCo;
    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
    }
    public override void ShowTooltip(bool show, RectTransform targetRect)
    {
        base.ShowTooltip(show, targetRect);
    }
    public void ShowTooltip(bool show, RectTransform targetRect, Skill_DataSO skillData,UI_TreeNode node)//同名函数重载，允许不同参数执行不同内容
    {
        base.ShowTooltip(show, targetRect);
        if (show == false)
            return;

        skillName.text = skillData.skillName;//技能名字文本
        skillDescription.text = $"{skillData.description}\n冷却时间：{skillData.upgradeData.cooldown}s";//描述文本

        if(node == null)
        {
            skillRequirements.text = "";
            return;
        }
        string skillLockedText = $"<color={importantInfoHex}>{lockedSkillText}</color>";//锁定文本
        string requirement = node.islocked ? skillLockedText : GetRequirements(node.skillData.cost, node.needNodes, node.conflictNodes);
        skillRequirements.text = requirement;
    }
    private string GetRequirements(int cost, UI_TreeNode[] needNodes, UI_TreeNode[] conflictNodes)//用于得到需求文本
    {
        StringBuilder sb = new StringBuilder();//字符串处理类
        sb.AppendLine("——需 求——");

        string costColor = skillTree.HaveEnoughSkillPoint(cost) ? meetConditionHex : NotMeetConditionHex;
        sb.AppendLine($"<color={costColor}>{cost} 技能点</color>");

        foreach (var node in needNodes)
        {
            if (node == null) continue;
            string nodeColor = node.isUnlocked ? meetConditionHex : NotMeetConditionHex;
            sb.AppendLine($"<color={nodeColor}>{node.skillData.skillName}</color>");
        }
        if (conflictNodes.Length <= 0)
            return sb.ToString();
        sb.AppendLine();//空行
        sb.AppendLine($"<color={importantInfoHex}>-冲突技能-</color>");
        foreach (var node in conflictNodes)
        {
            if (node == null) continue;
            sb.AppendLine($"<color={importantInfoHex}>{node.skillData.skillName}</color>");
        }
        return sb.ToString();
    }
    public void lockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCo(textEffectCo);
        textEffectCo = StartCoroutine(TextBlinkEffectCO(skillRequirements, 0.15f, 3));
    }
    public void StopCo(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    private IEnumerator TextBlinkEffectCO(TextMeshProUGUI text, float Interval, int Count)//使锁定文本闪烁
    {
        for (int i = 0; i < Count; i++)
        {
            text.text = GetColorText(WarningHex, lockedSkillText);
            yield return new WaitForSeconds(Interval);
            text.text = GetColorText(importantInfoHex, lockedSkillText);
            yield return new WaitForSeconds(Interval);
        }
    }
}
