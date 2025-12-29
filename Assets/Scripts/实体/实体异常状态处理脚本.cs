using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_stats EntityStats;
    private EntityHp entityHp; 
    private ElementType currentEffect = ElementType.None;

    [Header("闪电效果细节")]
    [SerializeField] private GameObject lightningVfx;//unity面板设定一个预制体
    [SerializeField] private float currentCharge;
    [SerializeField] private float maxCharge = 1;
    private Coroutine electrifyCo;
    private void Awake()
    {
        EntityStats = GetComponent<Entity_stats>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityHp = GetComponent<EntityHp>();
    }

    public void ApplyElectrifyEffect(float duration,float damage,float charge)
    {
        float lightningResistance = EntityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);
        currentCharge = currentCharge + finalCharge;
        if(currentCharge >= maxCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }
        if (electrifyCo != null)
            StopCoroutine(electrifyCo);
        electrifyCo = StartCoroutine(ElectrifyEffectCo(duration));//启动雷电闪烁协程
    }
    private void StopElectrifyEffect()//停止闪烁特效
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }
    private void DoLightningStrike(float damage)//创建雷暴特效与伤害
    {
        Instantiate(lightningVfx, transform.position, Quaternion.identity);//实例化（闪电预制体，位置，旋转）
        entityHp.ReduceHp(damage);
    }
    private IEnumerator ElectrifyEffectCo(float duration)//雷电闪烁效果协程
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        //启动雷电效果的闪烁，延迟几秒后结束雷电效果，在这期间会充能，达到界限会触发雷暴特效
        yield return new WaitForSeconds(duration);
        StopElectrifyEffect();
    }
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = EntityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }
    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;//锁定状态为火属性，阻止其他属性状态应用
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;//次数每秒
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);//次数需要是整数，RoundToInt实现四舍五入整数

        float damagePerTick = totalDamage / tickCount;//每次造成的伤害
        float tickInterval = 1f / ticksPerSecond;//避免整数除法，C#的整数除整数只会保留整数结果，可能会导致结果为0，所以加上f改为浮点数运算

        for (int i = 0; i < tickCount; i++)
        {
            entityHp.ReduceHp(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
        currentEffect = ElementType.None;//解除属性状态
    }
    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = EntityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance);//获取冰冻抗性并减少持续时间

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));//启动冰冻异常状态协程（减速+闪烁）
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
        if(element == ElementType.Lightning && currentEffect == ElementType.Lightning)//攻击元素为雷电且当前启动的属性为雷电时允许运用雷电效果
            return true;
        return currentEffect == ElementType.None;
        //目标身上无异常效果返回true，用于实体战斗-ApplyStatusEffect中判断是否能够给予异常效果
    }
}
