using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/物品数据/物品列表", fileName = "物品列表-")]

public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] itemList;
    public ItemDataSO GetItemData(string saveId)//通过id查找物品数据
    {
        return itemList.FirstOrDefault(item => item != null && item.savaId == saveId);
    }

#if UNITY_EDITOR
    [ContextMenu("点击自动填充所有物品数据")]
    public void CollectItemsData()//收集物品数据
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO");
        itemList = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))//根据路径加载ItemDataSO资产(括号中：将GUID转换为资源路径)
            .Where(item => item != null)//过滤掉加载失败的空对象
            .ToArray();//将结果转换为数组
        EditorUtility.SetDirty(this);//标记当前脚本对象已被修改
        AssetDatabase.SaveAssets();//保存所有未保存的资源修改到磁盘
    }
#endif
}
