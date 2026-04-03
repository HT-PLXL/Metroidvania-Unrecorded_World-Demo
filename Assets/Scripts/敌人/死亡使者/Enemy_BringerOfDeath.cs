using System.Collections;
using UnityEngine;

public class Enemy_BringerOfDeath : Enemy, ICounter
{
    public bool CanBeCountered { get => canBeStunned; }
    public BringerOfDeath_AttackState bringerOfDeath_AttackState { get; set; }
    public BringerOfDeath_BattleState bringerOfDeath_BattleState { get; set; }
    public BringerOfDeath_TeleportState bringerOfDeath_TeleportState { get; set; }
    public BringerOfDeath_SkillState bringerOfDeath_SkillState { get; set; }
    [Header("传送细节")]
    [SerializeField][cn("传送范围矩形")] private BoxCollider2D teleportRectangle;
    [SerializeField][cn("攻击后传送几率")] private float chanceToTeleport = 0.25f;
    [cn("最大战斗时待机时间")] public float maxBattleIdleTime = 2.5f;
    [Header("技能细节")]
    [SerializeField][cn("技能伤害倍率表")] DamageScaleFactor damageScaleFactor;
    [SerializeField][cn("技能预制体")] private GameObject skillPrefab;
    [SerializeField][cn("死亡动画预制体")] private GameObject deadPrefab;
    [SerializeField][cn("基础召唤攻击次数")] private int skillAmount = 6;
    [SerializeField][cn("召唤攻击间隔")] private float skillCastRate = 1f;
    [SerializeField][cn("技能冷却，注意施法即计时")] private float SkillCooldown = 20;
    [SerializeField][cn("预判走位偏移量")] private Vector2 playerOffset;
    private float lastTimeUseSkill = -999f;
    public bool useSkillPerformed { get; set; }
    private Player playerObject;


    private float offsetY;
    private float defaultTeleportChance;
    public bool teleportTrigger { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        bringerOfDeath_TeleportState = new BringerOfDeath_TeleportState(this, stateMachine, "teleport");
        bringerOfDeath_AttackState = new BringerOfDeath_AttackState(this, stateMachine, "attack");
        bringerOfDeath_SkillState = new BringerOfDeath_SkillState(this, stateMachine, "useSkill");
        bringerOfDeath_BattleState = new BringerOfDeath_BattleState(this, stateMachine, "battle");
        battleState = bringerOfDeath_BattleState;////地面状态索敌会转入battleState状态，所以需要battleState

        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        teleportRectangle.transform.parent = null;
        defaultTeleportChance = chanceToTeleport;
        offsetY = col.bounds.size.y / 2f;
    }
    protected override void Update()
    {
        base.Update();
        Debug.Log($"{stateMachine.currentState}");

    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }
    public override void SpecialAttack()
    {
        StartCoroutine(UseSkillCo());
    }
    private IEnumerator UseSkillCo()
    {
        if (playerObject == null)
            playerObject = player.GetComponent<Player>();

        Vector3 lastPosition = new Vector3 (0,0);
        for (int i = 0; i < skillAmount; i++)
        {
            bool playerMoving = playerObject.rb.linearVelocity.magnitude > 0;
            bool moveLittle = Mathf.Abs(lastPosition.x - playerObject.transform.position.x) < 2;

            float xOffset = playerMoving ? playerOffset.x * playerObject.faceDirection : 0;
            if (moveLittle)//移动距离太短额外惩罚,左
                xOffset = -0.9f;

            Vector3 spellPosition = player.transform.position + new Vector3(xOffset, playerOffset.y);
            BringerOfDeath_Skill skill = Instantiate(skillPrefab, spellPosition, Quaternion.identity).GetComponent<BringerOfDeath_Skill>();
            skill.SetSkillObject(combat, damageScaleFactor);

            if(moveLittle)//移动距离太短额外惩罚，右
            {
                spellPosition = player.transform.position + new Vector3(0.9f, playerOffset.y);
                BringerOfDeath_Skill skillTwo = Instantiate(skillPrefab, spellPosition, Quaternion.identity).GetComponent<BringerOfDeath_Skill>();
                skillTwo.SetSkillObject(combat, damageScaleFactor);
            }
            lastPosition = playerObject.transform.position;
            yield return new WaitForSeconds(skillCastRate);
        }
        SetUseSkillPerformed(true);
        Debug.Log("??!!!666");
    }
    public bool CanTeleport()
    {
        if (Random.value < chanceToTeleport)
        {
            chanceToTeleport = defaultTeleportChance;
            return true;
        }

        chanceToTeleport = chanceToTeleport + .05f;
        return false;
    }
    public bool CanUseSkill() => Time.time > lastTimeUseSkill + SkillCooldown;
    public void IntoCooldown() => lastTimeUseSkill = Time.time;
    public void SetUseSkillPerformed(bool SkillPreformed) => useSkillPerformed = SkillPreformed;
    public void SetTeleportTrigger(bool trigger) => teleportTrigger = trigger;
    public Vector3 FindTeleportPoint()
    {
        int maxAttempts = 10;
        float bossWithColliderHalf = col.bounds.size.x / 2;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(teleportRectangle.bounds.min.x + bossWithColliderHalf,
                                         teleportRectangle.bounds.max.x - bossWithColliderHalf);

            Vector2 raycastPoint = new Vector2(randomX, teleportRectangle.bounds.max.y);

            RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, whatIsGround);

            if (hit.collider != null)
                return hit.point + new Vector2(0, offsetY);
        }

        return transform.position;
    }
    private void DeadAnim()
    {
        Vector3 position = new Vector3(this.transform.position.x + 2, this.transform.position.y+0.5f);
        Instantiate(deadPrefab, position, deadPrefab.transform.localRotation);
    }
    public override void AfterDie()
    {
        DeadAnim();
        Destroy(gameObject, 1.2f);
    }
}
