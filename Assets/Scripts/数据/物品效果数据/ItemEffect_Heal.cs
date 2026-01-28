using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/低级治疗", fileName = "物品效果-低级治疗")]

public class ItemEffect_Heal :ItemEffectDataSO
{
    [SerializeField] private float healPercent = 0.1f;
    public override void ExecuteEffect()
    {
        Player player = FindFirstObjectByType<Player>();
        float healAmount = player.entityStats.GetMaxHp() * healPercent;
        player.Hp.IncreaseHp(healAmount);
    }
}
