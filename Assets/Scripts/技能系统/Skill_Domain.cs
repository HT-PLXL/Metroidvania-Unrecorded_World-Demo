using System.Collections.Generic;
using UnityEngine;

public class Skill_Domain : Skill_Base
{
    [Header("领域详情")]
    public float maxSize = 10;
    public float expandSpeed = 3;
    [SerializeField] private GameObject domainPrefab;
    [Header("时空迟滞领域")]
    [SerializeField] private float slowDownPercent = 0.8f;
    [SerializeField] private float slowDownDuration = 5;
    [Header("时空领域_虚妄意志")]
    [SerializeField] private int cloneCount =10;//克隆体数量
    [SerializeField] private float cloneUpgradeSlowDownPercent = 1f;
    [SerializeField] private float cloneUpgradeDuration = 8;
    [Header("时空领域_魔能轰炸")]
    [SerializeField] private int shardCount = 20;//魔能晶体数量
    [SerializeField] private float shardUpgradeSlowDownPercent = 1f;
    [SerializeField] private float shardUpgradeDuration = 6;
    //[SerializeField] private float damageHealPercent = 0.5f;
    private float spellCastTimer;
    private float spellCastPerSecond;

    private List<Enemy> Alltargets = new List<Enemy>();
    private Transform currentTarget;
    public void CreateDomain()
    {
        spellCastPerSecond = GetSpellCastCount() / GetDomainDuration();
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_Domain>().SetUpDomain(this, IsPhysicalDamage);
    }
    public void HandleSpellCast()//SpellCast法术释放
    {
        spellCastTimer -= Time.deltaTime;
        if (currentTarget == null)
            currentTarget = GetTarget();
        if(currentTarget != null && spellCastTimer<0)
        {
            SpellCast(currentTarget);
            spellCastTimer = 1 / spellCastPerSecond;
            currentTarget = null;
        }
    }
    private void SpellCast(Transform target)
    {
        if(upgradeType == SkillUpgradeType.时空领域_虚妄意志)
        {
            Vector3 offset = Random.value < 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);
            skillManager.clone.CreateClone(target.position + offset);
        }
        if (upgradeType == SkillUpgradeType.时空领域_魔能轰炸)
        {
            skillManager.shard.CreateOriginalShard(target, true);
        }
    }
    private Transform GetTarget()
    {
        Alltargets.RemoveAll(target => target == null || target.enemyHp.isDead);//移除其中为空或者已死亡的对象
        if (Alltargets.Count == 0)
            return null;
        int index = Random.Range(0, Alltargets.Count);
        return Alltargets[index].transform;

    }
    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.时空领域)
            return slowDownDuration;
        else if(upgradeType == SkillUpgradeType.时空领域_虚妄意志)
            return cloneUpgradeDuration;
        else if (upgradeType == SkillUpgradeType.时空领域_魔能轰炸)
            return shardUpgradeDuration;
        return 0;
    }
    public float GetSlowDownPercent()
    {
        if (upgradeType == SkillUpgradeType.时空领域)
            return slowDownPercent;
        else if (upgradeType == SkillUpgradeType.时空领域_虚妄意志)
            return cloneUpgradeSlowDownPercent;
        else if (upgradeType == SkillUpgradeType.时空领域_魔能轰炸)
            return shardUpgradeSlowDownPercent;
        return 0;
    }
    private int GetSpellCastCount()
    {
        if (upgradeType == SkillUpgradeType.时空领域_虚妄意志)
            return cloneCount;
        else if (upgradeType == SkillUpgradeType.时空领域_魔能轰炸)
            return shardCount;
        return 0;
    }
    public bool IsInstantDomain()
    {
        return upgradeType != SkillUpgradeType.时空领域_虚妄意志 && upgradeType != SkillUpgradeType.时空领域_魔能轰炸;
    }
    public void AddTarget(Enemy target)
    {
        Alltargets.Add(target);
    }
    public void ClearTarget()
    {
        foreach (var enemy in Alltargets)
            enemy.StopSlowDown();
        Alltargets = new List<Enemy>();
    }
}
