using UnityEngine;

public class Enemy_Slime : Enemy, ICounter
{
    public bool CanBeCountered { get => canBeStunned; }
    private Transform BattleTarget;
    [SerializeField][cn("子史莱姆预制体")] private GameObject childSlimePrefab;
    [SerializeField][cn("分裂数量")] private int splitCount = 2;
    [SerializeField][cn("出现时扩散速度")] private Vector2 velocityOnCreation;

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");

        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();

        if (BattleTarget != null)
        {
            stateMachine.Initialize(battleState);
            BattleTarget = null;
            return;
        }
        stateMachine.Initialize(idleState);
    }
    public override void EntityDead()
    {
        stateMachine.ChangeState(deadState);
    }
    public void HandleCounter()//实现反击接口
    {
        stateMachine.ChangeState(stunnedState);
    }
    /// <summary>
    /// 生成子史莱姆
    /// </summary>
    public void CreateChildSlime()
    {
        if (childSlimePrefab == null)
            return;
        for (int i = 0; i < splitCount; i++)
        {
            GameObject newSlime = Instantiate(childSlimePrefab, transform.position, Quaternion.identity);
            Enemy_Slime childSlime = newSlime.GetComponent<Enemy_Slime>();
            childSlime.SetSlime(velocityOnCreation);
            childSlime.stats.GetNewSlimeStat(base.stats.resources, base.stats.attack, base.stats.defense, 0.5f, 1.3f);
        }
    }
    public void SetSlime(Vector2 velocity)
    {
        float xVelocity = velocity.x * Random.Range(-2f, 2f);
        float yVelocity = velocity.y * Random.Range(1f, 3f);
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }
    public override void AfterDie()
    {
        CreateChildSlime();
    }
}
