using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private 实体视觉效果 entityVfx;
    private Entity_stats stats;
    private ElementType currentEffect= ElementType.None;

    private void Awake()
    {
        stats = GetComponent<Entity_stats>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<实体视觉效果>();
    }
    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        float iceResistance = stats.GetElementalResistance(ElementType.Ice);
        float reducedDuration = duration * (1 - iceResistance);//获取冰冻抗性并减少持续时间

        StartCoroutine(ChillEffectCo(reducedDuration, slowMultiplier));//启动冰冻异常状态协程（减速+闪烁）
    }
    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);//执行实体减速方法
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);//执行变色闪烁方法
        
        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }
    public bool CanApply(ElementType element)
    {
        return currentEffect == ElementType.None;
        //目标身上无异常效果返回true，用于实体战斗-ApplyStatusEffect中判断是否能够基于异常效果
    }
}
