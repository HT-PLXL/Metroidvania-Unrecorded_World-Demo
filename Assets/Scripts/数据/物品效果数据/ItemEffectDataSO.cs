using UnityEngine;

//提供一个描述槽，以及供子类继承的方法（执行效果）
public class ItemEffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    protected Player player;
    public virtual bool CanBeUsed()
    {
        return true;
    }
    public virtual void ExecuteEffect()
    {

    }
    public virtual void Subscribe(Player player)//订阅
    {
        this.player = player;
    }
    public virtual void Unsubscribe()//取消订阅
    {

    }
}
