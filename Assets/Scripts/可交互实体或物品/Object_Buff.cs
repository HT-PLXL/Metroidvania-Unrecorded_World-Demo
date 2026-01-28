using UnityEngine;

//关于Object_Buff：给予碰撞体，设置为is Trigger，将其设置为专属层级，再在unity设置里关闭除了与玩家之间的碰撞
//应用buff的协程在Player_Stats内
public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Player_Stats statsToModify;

    [Header("Buff细节")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4;
    [Header("漂浮细节")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;//y = sin（x * k） * 范围比例
        transform.position = startPosition + new Vector3(0, yOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)//与is Trigger对应的触发函数
    {
        statsToModify = collision.GetComponent<Player_Stats>();

        if (statsToModify.CanApplyBuff(buffName))
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }
    }
}
