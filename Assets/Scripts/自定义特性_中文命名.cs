// 1. 引入System命名空间，因为要使用Attribute（特性）相关的基类和枚举
using System;
// 2. 引入UnityEngine命名空间，因为要继承Unity的PropertyAttribute（属性特性基类）
using UnityEngine;

// 3. [AttributeUsage]：限制当前自定义特性的适用范围，核心配置如下
//    AttributeTargets.Field：仅允许用于「字段」（就是我们要序列化的变量，解决之前的报错问题）
//    Inherited = true：允许子类继承该特性
//    AllowMultiple = false：不允许同一个字段多次使用该特性（避免重复命名）
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
// 4. 定义自定义特性类，命名为cnAttribute，继承自PropertyAttribute（Unity所有字段特性的基类）
//    命名规范：特性类后缀必须是Attribute，使用时可以省略（后续直接写[cn("中文")]即可）
public class cnAttribute : PropertyAttribute
{
    // 5. 定义公共只读属性，命名精简为Name，用于存储中文显示名称
    //    get; private set;：外部只能读取，只能在类内部赋值（保证数据安全，避免外部篡改）
    public string Name { get; private set; }

    // 6. 构造函数（与类名同名），接收一个string类型的参数（就是我们要传入的中文名称）
    //    构造函数是特性的核心，使用[Name("中文")]时，实际就是调用这个构造函数传入参数
    public cnAttribute(string name)
    {
        // 7. 将构造函数接收的中文名称，赋值给上面定义的Name属性，完成数据存储
        this.Name = name;
    }
}
