using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/材料", fileName = "材料-")]
public class ItemDataSO : ScriptableObject
{
    public string savaId { get; private set; }
    //[cn("")]
    [cn("物品名")] public string itemName;
    [cn("图标")] public Sprite icon;
    [cn("物品种类")] public ItemType itemType;
    [cn("最大堆叠数量")] public int maxStackCount = 1;

    [Header("物品效果")]
    public ItemEffectDataSO itemEffect;
    [Header("掉落机制-滑动条设置稀有度+最大掉落率百分比%")]
    [Range(0,1000)] public int itemRarity = 100;
    [Range(0, 100)][cn("最大掉落几率")] public float maxDropChance = 75f;
    [cn("掉落率百分比%")] public float dropChance;
    [cn("品质")] public string level;

    [Header("商店详情")]
    [cn("购买价格")] public int itemPrice = 100;
    [cn("商店中每格最小数量")] public int minStackCountInShop = 1;
    [cn("商店中每格最大数量")] public int maxStackCountInShop = 1;
    [Header("制作配方")]
    public Bag_Item[] craftRecipe;

    public float GetDropChance()
    {
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100;
        return Mathf.Min(chance, maxDropChance);//可以强制设定最大掉落几率为一个较小数进行限制
    }

    private void OnValidate()//编辑器更新函数，即时展示掉落率
    {
        dropChance = GetDropChance();
        level = GetRarityLevel(itemRarity);
#if UNITY_EDITOR//编辑器模式下才执行
        string path = AssetDatabase.GetAssetPath(this);//得到资产路径
        savaId = AssetDatabase.AssetPathToGUID(path);//将资源路径转换为Unity内部的唯一标识符GUID（Global Unique Identifier）
#endif
    }

    public string GetRarityLevel(int rarity)//确定物品品质(用颜色区分)
    {
        if (rarity < 341)//对应 66.1% - 100% 掉落率
            return "白色普通";
        if (rarity < 501)//对应 50.1% - 66% 掉落率
            return "绿色优秀";
        if (rarity < 751)//对应 25.1% - 50% 掉落率
            return "蓝色稀有";
        if (rarity < 851)//对应 15.1% - 25% 掉落率
            return "紫色史诗";//紫色
        if (rarity < 951)//对应 5.1% - 15% 掉落率
            return "金色传说";//金色
        return "红色禁忌";    //对应 0.1% - 5% 掉落率
    }
}
