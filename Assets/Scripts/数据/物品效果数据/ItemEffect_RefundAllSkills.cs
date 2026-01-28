using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/技能树重置", fileName = "物品效果-技能树重置")]

public class ItemEffect_RefundAllSkills : ItemEffectDataSO
{
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.RefundAllSkillPoint();

        //node.RefundSkillPoint()的connectHandle后加了?空判定
        //使其在刚运行未打开过技能树的情况下也能使用遗忘之书，虽然有点抽象，但至少不报错了，逻辑也没问题，谁说没解锁技能就不能遗忘的(雾)
    }
}
