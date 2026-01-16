using System.Collections;
using UnityEngine;
//挂载在技能容器上
public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    [SerializeField] private EntityHp playerHp;
    [SerializeField][cn("碎片预制体")] private GameObject shardPerfab;
    [SerializeField][cn("引爆时间")] private float detonateTime = 2;
    [Header("磁力碎片升级")]
    [SerializeField] private float shardSpeed = 7;

    [Header("连锁碎片升级")]
    [SerializeField] private int maxCount = 3;
    [SerializeField] private int currentCount;
    [SerializeField] private bool isCharging;//正在充能

    [Header("超时空升级")]
    [SerializeField] private float shardDuration = 10;

    [Header("时空回溯升级")]
    [SerializeField] private float originalHp;
    protected override void Awake()
    {
        base.Awake();
        currentCount = maxCount;
        playerHp = GetComponentInParent<EntityHp>();
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
        if (Unlocked(SkillUpgradeType.超时空升级))
            HandleShardTeleport();
        if (Unlocked(SkillUpgradeType.时空回溯升级))
            HandleShardTeleportAndRegen();
    }
    private void HandleShardTeleportAndRegen()
    {
        if (currentShard == null)
        {
            CreateShard();
        }
        else
        {
            SwapPlayerAndShard();
            playerHp.IncreaseHp(0.50f * Mathf.Abs(originalHp - playerHp.currentHp));
            setSkillOnCooldown();
        }
    }
    private void HandleShardTeleport()//传送
    {
        if (currentShard == null)
        {
            CreateShard();
        }
        else
        {
            SwapPlayerAndShard();
            setSkillOnCooldown();
        }
    }
    private void SwapPlayerAndShard()//交换位置
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        player.TeleportPlayer(shardPosition);
        currentShard.Explode();
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
        originalHp = playerHp.currentHp;
        GameObject shard = Instantiate(shardPerfab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetUpShard(this);
        if (Unlocked(SkillUpgradeType.超时空升级) || Unlocked(SkillUpgradeType.时空回溯升级))
            currentShard.OnExplode += Cooldown;//订阅Cooldown()
    }
    public void CreateOriginalShard()
    {
        bool canMove = Unlocked(SkillUpgradeType.磁力升级 )|| Unlocked(SkillUpgradeType.充能升级);
        GameObject shard = Instantiate(shardPerfab, transform.position, Quaternion.identity);
        shard.GetComponent<SkillObject_Shard>().SetUpShard(this,detonateTime,canMove,shardSpeed);//调用重载的方法
    }

    public float GetDetonateTime()//选择存在时间
    {
        if (Unlocked(SkillUpgradeType.超时空升级) || Unlocked(SkillUpgradeType.时空回溯升级))
            return shardDuration;
        return detonateTime;
    }
    private void Cooldown()
    {
        if (OnCooldown() == false)
        {
            setSkillOnCooldown();
            currentShard.OnExplode -= Cooldown;
        }
    }
}
