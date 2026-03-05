using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品列表", fileName = "列表物品-")]

public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] itemList;
    public ItemDataSO GetItemData(string saveId)
    {
        return itemList.FirstOrDefault(item => item != null && item.savaId == saveId);
    }

#if UNITY_EDITOR
    [ContextMenu("自动填充所有物品数据")]
    public void CollectItemsData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO");
        itemList = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(item => item != null)
            .ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
