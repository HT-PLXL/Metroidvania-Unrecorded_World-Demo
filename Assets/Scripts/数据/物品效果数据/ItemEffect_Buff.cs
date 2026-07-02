using System;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/buff效果", fileName = "物品效果-buff")]

public class ItemEffect_Buff : ItemEffectDataSO
{
    [SerializeField] private BuffEffectData[] buffsNeedApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();//不重复的id

    public override bool CanBeUsed(Player player)
    {
        if (player.stats.CanApplyBuff(source))
        {
            this.player = player;
            return true;
        }
        Debug.Log("不能应用相同的buff");
        return false;
    }
    public override void ExecuteEffect()//执行效果
    {
        player.stats.ApplyBuff(buffsNeedApply, duration, source);
        player = null;
    }
}
