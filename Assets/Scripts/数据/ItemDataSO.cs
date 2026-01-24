using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/材料", fileName = "材料-")]
public class ItemDataSO : ScriptableObject
{
    //[cn("")]
    [cn("物品名")]public string itemName;
    [cn("图标")] public Sprite icon;
    [cn("物品种类")] public ItemType itemType;
    [cn("最大堆叠数量")] public int maxStackCount = 1;
}
