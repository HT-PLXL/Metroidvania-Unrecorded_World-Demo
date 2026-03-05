using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnAfterDeserialize()//在加载数据后取回两个列表并转换回字典
    {
        this.Clear();

        if (keys.Count != values.Count)
            Debug.Log("键与值数量不等");
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);

        }
    }

    public void OnBeforeSerialize()//在保存前将字典转化为两个列表,分别保存键与值
    {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<TKey, TValue>pair in this)//this继承自字典
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
