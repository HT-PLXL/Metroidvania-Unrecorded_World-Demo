using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Buff
{
    public StatType type;
    public float value;
}
//关于Object_Buff：给予碰撞体，设置为is Trigger，将其设置为专属层级，再在unity设置里关闭除了与玩家之间的碰撞
public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Buff细节")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4;
    [SerializeField] private bool canBeUsed = true;
    [Header("漂浮细节")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }
    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;//y = sin（x * k） * 范围比例
        transform.position = startPosition + new Vector3(0, yOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)//与is Trigger对应的触发函数
    {
        if(canBeUsed == false)
            return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear;//清除颜色使其不可见

        ApplyBuff(true);

        Debug.Log("给予buff" + duration + "秒");
        yield return new WaitForSeconds(duration);
        Debug.Log("Object_Buff 已清除");

        ApplyBuff(false);
        Destroy(gameObject);//在buff时间过后销毁对象
    }
    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);//增加属性
            else
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);//销毁buff属性
        }
    }
}
