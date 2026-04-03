#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// 自定义枚举的 PropertyDrawer
[CustomPropertyDrawer(typeof(StatType))]
public class StatTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            // 获取枚举类型和所有值
            string[] enumNames = property.enumDisplayNames;
            int currentIndex = property.enumValueIndex;

            // 转换为中文选项
            string[] chineseOptions = new string[enumNames.Length];
            Type enumType = fieldInfo.FieldType;

            for (int i = 0; i < enumNames.Length; i++)
            {
                string enumName = enumNames[i];
                if (Enum.TryParse(enumType, enumName, out object enumValue))
                {
                    chineseOptions[i] = ((Enum)enumValue).GetChineseName();
                }
                else
                {
                    chineseOptions[i] = enumName;
                }
            }

            // 显示下拉菜单
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, chineseOptions);
            if (newIndex != currentIndex)
            {
                property.enumValueIndex = newIndex;
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif