using System.Collections.Generic;
using UnityEngine;

public class SkillObject_BounceSword : SkillObject_Sword
{
    //[SerializeField][cn("")]
    [SerializeField][cn("弹射速度")] private float bounceSpeed = 15;
    [SerializeField][cn("弹射次数")] private int bounceNumber = 5;
    private Collider2D[] targetEnemy;
    private Transform nextTarget;
    private List<Transform> alreadyUsed = new List<Transform>();
    public override void SetUpSword(Skill_ThrowSword throwSword, Vector2 direction)//接收技能容器设置的值覆盖默认值
    {
        anim.SetTrigger("飞剑旋转");//动画节点触发掷剑生成后立即设定动画
        base.SetUpSword(throwSword, direction);
        bounceSpeed = throwSword.bounceSpeed;
        bounceNumber = throwSword.bounceNumber;
        checkRadius = throwSword.checkRadius;
    }
    protected override void Update()
    {
        HandleComeBack();
        HandleBounce();
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (targetEnemy == null)
        {
            targetEnemy = AroundEnemy(transform, checkRadius);
            rb.simulated = false;
        }
        DamageEnemyInRadius(transform, 1);
        if (targetEnemy.Length <= 1 || bounceNumber == 0)//初次检测目标少于等于1个或次数设定为0
            ShouldComeBack();
        //后续可实现单体高伤害完善
        else
            nextTarget = GetNextTarget();
    }
    private void HandleBounce()
    {
        if (nextTarget == null)//有目标才继续执行
            return;
        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, nextTarget.position) < 1f)
        {
            DamageEnemyInRadius(transform, 1);
            BounceNextTarget();
            if (bounceNumber == 0 || nextTarget == null)//次数用完/目标清空
            {
                nextTarget = null;//重置，为下次技能做准备
                ShouldComeBack();//返回玩家
            }
        }
    }
    private void BounceNextTarget()
    {
        Transform newTarget = GetNextTarget();
        // 只有获取到有效目标时，才递减次数
        if (newTarget != null)
        {
            Debug.Log($"弹射次数递减：当前剩余次数={bounceNumber - 1}，目标={newTarget.name}");
            nextTarget = newTarget;
            bounceNumber--;
        }
        else
        {
            nextTarget = null;
        }
    }
    private Transform GetNextTarget()//得到下个有效存活目标
    {
        List<Transform> effectiveTarget = GetEffectiveTarget();
        if (effectiveTarget.Count == 0)
            return null;
        int index = Random.Range(0, effectiveTarget.Count);//随机生成索引
        Transform nextTarget = effectiveTarget[index];//确定目标
        alreadyUsed.Add(nextTarget);//置入已选列表，目标循环完一遍前不会重复选择
        return nextTarget;
    }
    private List<Transform> GetEffectiveTarget()//得到所有有效存活目标
    {
        List<Transform> effectiveTargets = new List<Transform>();
        List<Transform> aliveTarget = GetAliveTarget();
        foreach (var enemy in aliveTarget)
        {
            if (enemy != null && alreadyUsed.Contains(enemy.transform) == false)//Contains判断是否包含，返回bool值
                effectiveTargets.Add(enemy.transform);
        }
        //if (effectiveTargets.Count > 0)
        //    return effectiveTargets;
        //else
        //{
        //    alreadyUsed.Clear();
        //    return aliveTarget;
        //}
        if (effectiveTargets.Count == 0)
        {
            alreadyUsed.Clear();
            foreach (var enemy in aliveTarget)
            {
                if (enemy != null && enemy != nextTarget)
                    effectiveTargets.Add(enemy);
            }
        }
        return effectiveTargets;
    }
    private List<Transform> GetAliveTarget()//得到所有存活目标
    {
        List<Transform> alivetarget = new List<Transform>();
        foreach (var enemy in targetEnemy)
        {
            if (enemy != null)
                alivetarget.Add(enemy.transform);
        }
        return alivetarget;
    }

}
