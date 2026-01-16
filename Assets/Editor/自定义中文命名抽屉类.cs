//// 1. 引入UnityEditor命名空间，包含所有编辑器拓展相关的API（仅在编辑器环境生效，打包时会被忽略）
//using UnityEditor;
//// 2. 引入UnityEngine命名空间，用于使用GUI相关的类（如Rect、GUIContent）
//using UnityEngine;

//// 3. [CustomPropertyDrawer]：将当前抽屉类与我们自定义的NameAttribute特性绑定
////    意思是：当Unity编辑器遇到带有[Name]特性的字段时，就会调用这个抽屉类来渲染面板
//[CustomPropertyDrawer(typeof(cnAttribute))]
//// 4. 定义抽屉类，命名精简为NameDrawer，继承自PropertyDrawer（Unity所有字段抽屉类的基类）
////    抽屉类的核心作用是自定义字段在Inspector面板的显示样式
//public class NameDrawer : PropertyDrawer
//{
//    // 5. 重写OnGUI方法（强制重写，核心渲染方法）
//    //    该方法会在Unity编辑器刷新Inspector面板时被调用
//    //    参数说明：
//    //    - position：字段在面板上的显示区域（矩形区域，包含坐标、宽高）
//    //    - property：当前被序列化的字段（包含字段的所有信息，如值、类型、名称等）
//    //    - label：字段的默认标签（原本的英文字段名，如canFade）
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // 6. 将当前抽屉类的attribute属性转换为NameAttribute类型
//        //    attribute是PropertyDrawer的内置属性，存储了绑定的特性实例（就是我们给字段加的[Name("中文")]）
//        //    转换后才能获取到我们之前存储的中文名称
//        cnAttribute nameAttr = (cnAttribute)this.attribute;

//        // 7. 创建一个自定义的GUIContent实例（GUI标签），用于替换默认的英文标签
//        //    传入nameAttr.Name，也就是我们之前存储的中文名称，完成标签文本替换
//        GUIContent customLabel = new GUIContent(nameAttr.Name);

//        // 8. 调用Unity编辑器的内置方法，绘制字段面板
//        //    position：使用默认的显示区域（保证字段排版整齐，不混乱）
//        //    property：当前被序列化的字段（保留字段的原有功能，如布尔值的勾选、浮点值的输入）
//        //    customLabel：使用我们自定义的中文标签（核心：替换英文为中文）
//        //    true：自动适配字段的高度（如数组、自定义类字段，避免显示不全）
//        EditorGUI.PropertyField(position, property, customLabel, true);
//    }
//}



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