using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    [SerializeField][cn("碎片预制体")] private GameObject shardPerfab;
    [SerializeField][cn("引爆时间")] private float detonateTime = 2;
    [Header("磁力碎片升级")]
    [SerializeField] private float shardSpeed = 7;

    [Header("连锁碎片升级")]
    [SerializeField] private int maxCount = 3;
    [SerializeField] private int currentCount;
    [SerializeField] private bool isCharging;//正在充能

    protected override void Awake()
    {
        base.Awake();
        currentCount = maxCount;
    }
    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;
        if (Unlocked(SkillUpgradeType.不稳定魔能晶体))
            HandleShard();
        if (Unlocked(SkillUpgradeType.磁力升级))
            HandleShareMoving();
        if (Unlocked(SkillUpgradeType.充能升级))
            HandleShardSplit();
    }
    private void HandleShardSplit()//split分裂
    {
        if (currentCount <= 0)
            return;
        CreateShard();
        currentShard.MoveToClosestTarget(shardSpeed);
        currentCount--;
        if (isCharging == false)
            StartCoroutine(ShardChargeCo());
    }
    private IEnumerator ShardChargeCo()
    {
        isCharging = true;
        while (currentCount < maxCount)
        {
            yield return new WaitForSeconds(cooldown);
            currentCount++;
        }
        isCharging = false;

    }
    private void HandleShareMoving()
    {
        CreateShard();
        currentShard.MoveToClosestTarget(cooldown);
        setSkillOnCooldown();
    }
    private void HandleShard()
    {
        CreateShard();
        setSkillOnCooldown();
    }

    public void CreateShard()
    {
        GameObject shard = Instantiate(shardPerfab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetUpShard(detonateTime);
    }
}
