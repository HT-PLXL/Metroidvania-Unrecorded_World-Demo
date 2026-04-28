using System.Collections.Generic;
using UnityEngine;

//**********自定义的可序列化字典，解决unity不能序列化字典的问题————通过将字典拆成两个列表再还原的方式**********

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver//序列化回调接收器接口 提供下面两个回调方法，使得能控制对象在序列化前和反序列化后的行为
{

    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnAfterDeserialize()//在加载数据后取回两个列表并转换回字典
    {
        Clear();
        if (keys.Count != values.Count)
        {
            Debug.Log("键与值数量不等");

        }
        for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
    }

    public void OnBeforeSerialize()//在保存前将字典转化为两个列表,分别保存键与值
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)//this继承自字典
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
