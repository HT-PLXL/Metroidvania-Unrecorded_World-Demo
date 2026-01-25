using System;
using System.Reflection;

    //获取枚举值的中文名称
public static class CnAttributeExtensions
{
    public static string GetChineseName(this Enum enumValue)
    {
        if (enumValue == null) return string.Empty;

        Type type = enumValue.GetType();
        string enumName = enumValue.ToString();

        // 获取对应的字段信息
        FieldInfo field = type.GetField(enumName);
        if (field == null) return enumName;

        // 获取 cn 特性
        cnAttribute attribute = field.GetCustomAttribute<cnAttribute>();
        return attribute?.ChineseName ?? enumName;
    }
}