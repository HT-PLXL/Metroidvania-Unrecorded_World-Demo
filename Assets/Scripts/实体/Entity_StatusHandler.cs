using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats EntityStats;
    private EntityHp entityHp; 
    private ElementType currentEffect = ElementType.None;

    [Header("闪电效果细节")]
    [SerializeField] private GameObject lightningVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maxCharge = 1;
    [SerializeField] private float lightningDamageMultiplier = 5;//使用的是目标身上的数值
    private Coroutine lightningCo;

    // 火视觉效果（独立管理，避免协程冲突）
    private Coroutine burnVfxRoutine;
    private float burnVfxEndTime;


    private void Awake()
    {
        EntityStats = GetComponent<Entity_Stats>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityHp = GetComponent<EntityHp>();
    }
    public void RemoveAllDebuff()
    {
        StopAllCoroutines();//关闭当前脚本所挂载的对象上由该脚本实例启动的所有协程
        currentEffect = ElementType.None;
        entityVfx.StopAllVfx();

        if (burnVfxRoutine != null)
        {
            StopCoroutine(burnVfxRoutine);
            burnVfxRoutine = null;
        }
        // 确保颜色重置
        entityVfx.SetSpriteColor(Color.white);
    }
    public void ApplyStatusEffect(ElementType element,ElementEffectData effectData)//用于应用元素伤害与效果
    {
        if (element == ElementType.Ice && CanApply(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMulitiplier);
        if (element == ElementType.Fire)
            ApplyBurnEffect(effectData.burnDuration, effectData.totalBurnDamage);
        if (element == ElementType.Lightning && CanApply(ElementType.Lightning))
            ApplyLightningEffect(effectData.lightningDuration, effectData.lightningDamage, effectData.lightningCharge);
    }

    public void ApplyLightningEffect(float duration,float damage,float charge)
    {
        float lightningResistance = EntityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);
        currentCharge = currentCharge + finalCharge;//通过传入的charge更新当前charge层数
        if (currentCharge >= maxCharge)
        {
            DoLightningStrike(damage * lightningDamageMultiplier);//应用雷暴并使用伤害乘数造成伤害
            StopLightningEffect();
            return;
        }
        if (lightningCo != null)
            StopCoroutine(lightningCo);
        lightningCo = StartCoroutine(LightningEffectCo(duration));//启动雷电闪烁协程
    }
    private void StopLightningEffect()//停止闪烁特效
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx();
    }
    private void DoLightningStrike(float damage)//创建雷暴特效与伤害
    {
        Instantiate(lightningVfx, transform.position, Quaternion.identity);//实例化（闪电预制体，位置，旋转）
        entityHp.ReduceHp(damage,ElementType.Lightning);
        // 显示雷电伤害跳字
        DamagePopupManager.Instance?.ShowDamage(transform.position + Vector3.up * 1f, damage, ElementType.Lightning, false, transform);
    }
    private IEnumerator LightningEffectCo(float duration)//雷电闪烁效果协程
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        //启动雷电效果的闪烁，延迟几秒后结束雷电效果，在这期间会充能，达到界限会触发雷暴特效
        yield return new WaitForSeconds(duration);
        StopLightningEffect();
    }
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = EntityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);

        // 灼烧伤害协程（每次攻击独立启动，允许叠加）
        StartCoroutine(BurnDamageCo(duration, finalDamage));

        // 火视觉效果：唯一，只刷新结束时间，不重启协程
        burnVfxEndTime = Time.time + duration;
        if (burnVfxRoutine == null)
            burnVfxRoutine = StartCoroutine(BurnVfxLoop());
    }
    // 独立的灼烧伤害协程
    private IEnumerator BurnDamageCo(float duration, float totalDamage)
    {
        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);
        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            entityHp.ReduceHp(damagePerTick, ElementType.Fire);
            DamagePopupManager.Instance?.ShowDamage(transform.position + Vector3.up * 1f, damagePerTick, ElementType.Fire, false, transform);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    // 火视觉效果循环（不依赖 Entity_VFX 的协程，仅控制颜色）
    private IEnumerator BurnVfxLoop()
    {
        SpriteRenderer sr = entityVfx.GetSpriteRenderer();
        if (sr == null) yield break;

        // 使用与 Entity_VFX 中 burnVfx 相同的颜色
        Color burnColor;
        ColorUtility.TryParseHtmlString("#EE7100", out burnColor);
        Color lightColor = burnColor * 1.05f;
        Color darkColor = burnColor * 0.95f;
        bool toggle = false;
        float tick = 0.25f;

        while (Time.time < burnVfxEndTime)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            yield return new WaitForSeconds(tick);
        }

        // 闪烁结束，恢复白色
        sr.color = Color.white;
        burnVfxRoutine = null;
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
            entityHp.ReduceHp(damagePerTick, ElementType.Fire);
            // 显示燃烧伤害跳字（跟随自身）
            DamagePopupManager.Instance?.ShowDamage(transform.position + Vector3.up * 1f, damagePerTick, ElementType.Fire, false, transform);
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
