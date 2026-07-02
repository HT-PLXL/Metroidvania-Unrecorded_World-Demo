using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品效果/获得100金币", fileName = "物品效果-100金币")]
public class ItemEffect_Get100Gold : ItemEffectDataSO
{
    [SerializeField] private int GoldNeedAdd;
    public override void ExecuteEffect()
    {
        Bag_Player bag = Player.instance.GetComponent<Bag_Player>();
        bag.AddGold(GoldNeedAdd);
    }
}
