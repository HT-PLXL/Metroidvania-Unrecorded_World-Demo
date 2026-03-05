using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/材料", fileName = "材料-")]
public class ItemDataSO : ScriptableObject
{
    public string savaId {  get;private set; }
    //[cn("")]
    [cn("物品名")] public string itemName;
    [cn("图标")] public Sprite icon;
    [cn("物品种类")] public ItemType itemType;
    [cn("最大堆叠数量")] public int maxStackCount = 1;

    [Header("物品效果")]
    public ItemEffectDataSO itemEffect;



    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        savaId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
