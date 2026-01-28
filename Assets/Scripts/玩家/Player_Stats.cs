using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//此类的一大作用是应用buff效果，可编程对象（物品效果）不能使用协程
public class Player_Stats : Entity_Stats
{
    private Bag_Player bag;
    private List<string> activeBuff = new List<string>();
    protected override void Awake()
    {
        bag = GetComponent<Bag_Player>();
    }
    public bool CanApplyBuff(string source)
    {
        return activeBuff.Contains(source) == false;//如果正在生效的buff的列表中不包含尝试使用的buff，则返回true
    }
    public void ApplyBuff(BuffEffectData[]buffNeedApply ,float duration,string source)
    {
        StartCoroutine(BuffCo(buffNeedApply, duration, source));
    }
    private IEnumerator BuffCo(BuffEffectData[] buffNeedApply, float duration, string source)
    {
        activeBuff.Add(source);
        foreach(var buff in buffNeedApply)
        {
            GetStatByType(buff.type).AddModifier(buff.value, source);
        }
        yield return new WaitForSeconds(duration);
        foreach (var buff in buffNeedApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);
        }
        bag.TriggerUpdateUI();
        activeBuff.Remove(source);
    }
}
