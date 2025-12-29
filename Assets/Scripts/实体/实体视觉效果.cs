using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;//渲染器
    private Entity entity;

    [Header("受伤视觉效果")]
    [SerializeField] private Material 受伤材质;
    [SerializeField] private float 受伤视觉效果持续时间 = 0.2f;

    private Material originalMaterial;
    private Coroutine 受伤视觉协程;
    private Coroutine statsCo;


    [Header("攻击特效")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;
    [SerializeField] private Color hitVfxColor = Color.white;

    [Header("元素颜色")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electrifyVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }
    public void PlayOnStatusVfx(float duration, ElementType element)//用于被调用启动异常状态视觉效果协程
    {
        if (element == ElementType.Ice)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
        if (element == ElementType.Fire)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
        if (element == ElementType.Lightning)
            statsCo = StartCoroutine(PlayStatusVfxCo(duration, electrifyVfx));
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

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.95f;

        bool toggle = false;

        while(timeHasPassed < duration)//条件满足时重复且延迟切换颜色深浅，实现闪烁
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            yield return new WaitForSeconds(tickInterval);
            timeHasPassed = timeHasPassed + tickInterval;//会有一定的时间误差，持续时间最好设置为间隔时间的倍数
        }
        //statsCo = null;
        sr.color = Color.white;//循环结束后设置为默认颜色
    }
    public void 启用攻击特效(Transform target, bool isCrit)//transform包含了位置，旋转，缩放
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;//选择攻击特效预制体
        GameObject vfx = Instantiate(hitPrefab, target.position,Quaternion.identity);
        //动态创建对象实例(unity中预制的对象，位置，旋转（四元数.无旋转）)
        if(isCrit == false)//将暴击特效排除在颜色修改之外
            vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;

        if (entity.面对方向 == -1 && isCrit)//通过GetComponent获取并赋给entity的组件是同一个GameObject（玩家）上的，自然只与玩家的方向有关而与敌人的方向无关
            vfx.transform.Rotate(0, 180, 0);
    }

    public void UpdataOnHitColor(ElementType element)
    {
        if (element == ElementType.Ice)
            hitVfxColor = chillVfx;
        if (element == ElementType.None)
            hitVfxColor = originalHitVfxColor;
    }

    private IEnumerator 受伤视觉效果转变()
    {
        sr.material = 受伤材质;
        yield return new WaitForSeconds(受伤视觉效果持续时间);
        sr.material = originalMaterial;
    }
    public void 播放受伤视觉效果()
    {
        if (受伤视觉协程 != null)
            StopCoroutine(受伤视觉协程);
        //连续受击，前面的协程会在后面的运行过程中就把材质转变为默认材质，因此需要停止
        受伤视觉协程 = StartCoroutine(受伤视觉效果转变());//启动IEnumerator
    }

}
