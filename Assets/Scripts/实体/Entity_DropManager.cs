using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//掉落物管理器
public class Entity_DropManager : MonoBehaviour
{
    [SerializeField][cn("掉落物预制体")] private GameObject itemDropPrefab;
    [SerializeField][cn("可掉落物列表")] private ItemListDataSO dropData;
    [SerializeField][cn("剩余稀有度点数")] private int maxRarityAmount = 1200;
    [SerializeField][cn("最大掉落数量")] private int maxItemsToDrop = 3;
    public virtual void DropItems()
    {
        List<ItemDataSO> itemsWillDrop = RollDrops();//执行掉落roll点，得到候选掉落列表
        int amountWillDrop = Mathf.Min(itemsWillDrop.Count, maxItemsToDrop);//取“候选列表数量”和“最大掉落数”的较小值（避免超出上限）
        //生成掉落物
        for (int i = 0; i < amountWillDrop; i++)
        {
            CreateItemDrop(itemsWillDrop[i]);
        }
    }
    protected void CreateItemDrop(ItemDataSO itemWillDrop)//生成掉落物
    {
        Vector3 offset = new Vector3(0, 1, 0);//一点偏移量，防止刚出来就触地导致不能正常扩散
        GameObject newItem = Instantiate(itemDropPrefab, transform.position + offset, Quaternion.identity);
        newItem.GetComponent<PickableItem>().SetupItem(itemWillDrop);
    }
    public List<ItemDataSO> RollDrops()//根据掉落率roll点，候选物品会按稀有度降序降序，再根据实体的剩余稀有度点数逐个判断能否获取
    {
        List<ItemDataSO> possibleDrops = new List<ItemDataSO>();// 候选掉落列表
        List<ItemDataSO> finalDrops = new List<ItemDataSO>();// 最终掉落列表

        foreach (var item in dropData.itemList)
        {
            float dropChance = item.GetDropChance();// 获取物品自身的掉落概率
            if (Random.Range(0, 100) <= dropChance)// 随机数0-100，如果小于等于掉落概率，就加入候选列表
                possibleDrops.Add(item);
        }
        //将候选物品按稀有度从高到低排序（优先掉落高稀有度物品）
        possibleDrops = possibleDrops.OrderByDescending(item => item.itemRarity).ToList();
        //按稀有度上限限制，筛选最终掉落列表
        foreach (var item in possibleDrops)
        {   // 如果剩余稀有度上限 > 当前物品的稀有度，就加入最终列表
            if (maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                maxRarityAmount = maxRarityAmount - item.itemRarity;//扣除已用的稀有度
            }
        }
        return finalDrops;
    }
}
