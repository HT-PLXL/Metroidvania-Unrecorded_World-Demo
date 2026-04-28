using System;
using UnityEditor;
using UnityEngine;

public class EnemyIdGenerator : EditorWindow
{
    [MenuItem("Tools/生成当前场景所有Enemy的ID")]
    private static void GenerateEnemyIds()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int count = 0;
        var processed = new System.Collections.Generic.HashSet<string>();

        foreach (var enemy in enemies)
        {
            if (string.IsNullOrEmpty(enemy.GetEnemyId()))
            {
                // 使用场景名 + 游戏物体名 + 唯一 GUID 组合，确保稳定且唯一
                string sceneName = enemy.gameObject.scene.name;
                string newId = $"{sceneName}_{enemy.gameObject.name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                // 通过反射设置私有字段或公开方法
                var so = new SerializedObject(enemy);
                var prop = so.FindProperty("enemyId");
                prop.stringValue = newId;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(enemy);
                count++;
            }
        }

        Debug.Log($"已为 {count} 个敌人自动生成存档 ID。");
        if (count > 0)
            AssetDatabase.SaveAssets();
    }
}