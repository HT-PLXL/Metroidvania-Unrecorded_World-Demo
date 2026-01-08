using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    public override void showTooltip(bool show, RectTransform targetRect)
    {
        base.showTooltip(show, targetRect);
    }
    public void showTooltip(bool show, RectTransform targetRect, Skill_DataSO skillData)//同名函数重载，允许不同参数执行不同内容
    {
        base.showTooltip(show, targetRect);
        if (show == false)
            return;

        skillName.text = skillData.skillName;
        skillDescription.text = skillData.description;
        skillRequirements.text = "——需求——\n" + skillData.cost + " 技能点";
    }

}
