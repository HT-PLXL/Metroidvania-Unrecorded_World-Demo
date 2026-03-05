using System;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/buff效果", fileName = "物品效果-buff")]

public class ItemEffect_Buff : ItemEffectDataSO
{
    [SerializeField] private BuffEffectData[] buffsNeedApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();//不重复的id
    private Player_Stats playerStats;

    public override bool CanBeUsed()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<Player_Stats>();

        if (playerStats.CanApplyBuff(source))
            return true;
        else
        {
            Debug.Log("不能应用相同的buff");
            return false;
        }
    }
    public override void ExecuteEffect()//执行效果
    {
        playerStats.ApplyBuff(buffsNeedApply, duration, source);
    }
}
