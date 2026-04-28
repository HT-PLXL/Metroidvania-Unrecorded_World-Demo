using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/吸血", fileName = "物品效果-吸血")]

public class ItemEffectOnTakingDamage : ItemEffectDataSO
{
    [SerializeField][cn("吸血比例")] private float healPercentOnAttack = 0.1f;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.playerCombat.OnTakingPhysicalDamage += HealOnTakingDamage;//伤害由事件顺带传递
    }
    public override void Unsubscribe()
    {
        if (player != null && player.playerCombat != null)
            player.playerCombat.OnTakingPhysicalDamage -= HealOnTakingDamage;
        base.Unsubscribe();
        player = null;//清除类对玩家实例的引用，使得实例可被回收释放内存
    }
    private void HealOnTakingDamage(float damage)
    {
            player.hp.IncreaseHp(damage * healPercentOnAttack);
    }
}
