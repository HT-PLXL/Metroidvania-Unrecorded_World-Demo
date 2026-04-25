using UnityEngine;

public class InstantKillSpike : SpikeBase
{
    [Header("即死伤害")]
    [SerializeField] private float killDamage = 99999; // 足以致死的高额伤害

    protected override void ApplyDamage(IDamageable target)
    {
        // 传递极大伤害，利用原有死亡逻辑
        target.TakeDamage(killDamage, 0, ElementType.None, transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }
}