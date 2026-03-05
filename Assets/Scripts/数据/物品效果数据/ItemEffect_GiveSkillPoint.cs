using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/技能点", fileName = "物品效果-技能点")]

public class ItemEffect_GiveSkillPoint : ItemEffectDataSO
{
    [SerializeField] private int PointNeedAdd;
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.AddSkillPoint(PointNeedAdd);
    }
}
