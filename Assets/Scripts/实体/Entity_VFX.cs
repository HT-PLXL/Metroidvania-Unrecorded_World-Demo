using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;//渲染器
    private Entity entity;

    [Header("受伤视觉效果")]
    [SerializeField] private Material beAttackMaterial;
    [SerializeField] private float beAttackVFXDuration = 0.2f;

    private Material originalMaterial;
    private Coroutine beAttackVFXCo;
    private Coroutine statsCo;

    [Header("攻击特效")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("元素颜色")]
    [SerializeField] private Color chillVfx;
    [SerializeField] private Color burnVfx;
    [SerializeField] private Color lightningVfx;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        ColorUtility.TryParseHtmlString("#009AFF", out chillVfx);
        ColorUtility.TryParseHtmlString("#FFD700", out lightningVfx);
        ColorUtility.TryParseHtmlString("#EA5420", out burnVfx);
    }
    public void PlayOnStatusVfx(float duration, ElementType element)//用于被调用启动异常状态视觉效果协程
    {
        if (element == ElementType.Ice)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
        if (element == ElementType.Fire)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
        if (element == ElementType.Lightning)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, lightningVfx));
    }
    public void StopAllVfx()
    {
        sr.color = Color.white;
        sr.material = originalMaterial;
        if (statsCo != null)
            StopCoroutine(statsCo);
    }
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;//短暂时间间隔
        float timeHasPassed = 0f;

        Color lightColor = effectColor * 1.1f;
        Color darkColor = effectColor * 0.95f;

        bool toggle = false;

        while (timeHasPassed < duration)//条件满足时重复且延迟切换颜色深浅，实现闪烁
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;//会有一定的时间误差，持续时间最好设置为间隔时间的倍数
        }
        sr.color = Color.white;//循环结束后设置为默认颜色
    }
    public void CreateHitVfx(Transform target, bool isCrit, ElementType element)//transform包含了位置，旋转，缩放
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;//根据是否暴击选择暴击特效还是普通打击特效(粒子特效)
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);//动态创建对象实例(unity中预制的对象，位置，旋转（四元数.无旋转）)
        if (isCrit == false)//将暴击特效排除在颜色修改之外，固定颜色
            vfx.GetComponentInChildren<SpriteRenderer>().color = GetHitVfxColor();//此处用于给打击特效上色,子类可重写GetHitVfxColor()，比如敌人重写默认灰色
        if (entity.faceDirection == -1 && isCrit)//通过GetComponent获取并赋给entity的组件是同一个GameObject（玩家）上的，自然只与玩家的方向有关而与敌人的方向无关
            vfx.transform.Rotate(0, 180, 0);
    }

    public virtual Color GetHitVfxColor()//子类可重写（如敌人默认灰）
    {
        return Color.white;
    }
    public Color GetHitVfxColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice:
                return chillVfx;
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Lightning:
                return lightningVfx;
            default:
                ColorUtility.TryParseHtmlString("#009AFF", out Color color);
                return color;
        }
    }

    private IEnumerator BeAttackVFXSwitch()
    {
        sr.material = beAttackMaterial;
        yield return new WaitForSeconds(beAttackVFXDuration);
        sr.material = originalMaterial;
    }
    public void PlayBeAttackVFX(ElementType element)
    {
        if (beAttackVFXCo != null)
            StopCoroutine(beAttackVFXCo);
        //连续受击，前面的协程会在后面的运行过程中就把材质转变为默认材质，因此需要停止
        if (element != ElementType.None)
            return;
        beAttackVFXCo = StartCoroutine(BeAttackVFXSwitch());//启动IEnumerator
    }

}
