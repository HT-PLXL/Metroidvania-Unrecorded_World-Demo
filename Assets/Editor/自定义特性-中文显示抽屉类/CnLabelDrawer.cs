using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(cnAttribute))]
public class CnLabelDrawer : PropertyDrawer
{
    // 保留Unity原生高度计算，确保嵌套类/多内容变量格式正常
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 传入原始label，让Unity正常计算多内容变量（嵌套类/数组）的折叠/展开高度
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. 获取自定义中文特性
        cnAttribute cnAttr = (cnAttribute)attribute;

        // 2. 关键：直接替换原生标签的文本（不破坏标签的其他属性，保留原生布局）
        // 仅修改text，tooltip等其他属性保持不变，多内容变量格式不受影响
        label.text = cnAttr.ChineseName;

        // 3. 用Unity原生方法完整渲染字段（保留所有格式：嵌套类折叠、TextArea、数组布局等）
        // 第三个参数true：保留多内容变量的折叠/展开功能，不破坏原有格式
        EditorGUI.PropertyField(position, property, label, true);
    }
}