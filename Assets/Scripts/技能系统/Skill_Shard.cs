using System.Collections;
using UnityEngine;
//挂载在技能容器上，下方声明的参数用来在技能容器修改对应技能实体脚本声明的参数的值
public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private EntityHp playerHp;
    [SerializeField][cn("水晶预制体")] private GameObject shardPerfab;
    [SerializeField][cn("引爆时间")] private float detonateTime = 2;
    [Header("磁力升级")]
    [SerializeField] [cn("飞行速度")]private float shardSpeed = 7;

    [Header("充能升级")]
    [SerializeField] [cn("最大数量")]private int maxNumber = 3;
    [SerializeField] [cn("当前可用数量")]private int currentNumber;
    [SerializeField] [cn("正在充能？")]private bool isCharging;//正在充能

    [Header("超时空升级")]
    [SerializeField] [cn("持续时间")]private float shardDuration = 10;

    [Header("时空回溯升级")]
    [SerializeField][cn("使用前血量")] private float originalHp;
    protected override void Awake()
    {
        base.Awake();
        currentNumber = maxNumber;
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
        if (currentNumber <= 0)
            return;
        CreateShard();
        currentShard.MoveToClosestTarget(shardSpeed);
        currentNumber--;
        if (isCharging == false)
            StartCoroutine(ShardChargeCo());
    }
    private IEnumerator ShardChargeCo()
    {
        isCharging = true;
        while (currentNumber < maxNumber)
        {
            yield return new WaitForSeconds(cooldown);
            currentNumber++;
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
