using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private ElementType currentEffect= ElementType.None;

    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        Debug.Log("冰冻效果已应用");
    }
    public bool CanApply(ElementType element)
    {
        return currentEffect == ElementType.None;
        //目标身上无异常效果返回true，用于实体战斗-ApplyStatusEffect中判断是否能够基于异常效果
    }
}
