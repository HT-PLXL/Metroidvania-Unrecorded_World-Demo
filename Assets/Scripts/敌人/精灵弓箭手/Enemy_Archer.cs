using UnityEngine;

public class Enemy_Archer : Enemy
{
    public bool CanBeCountered { get => canBeStunned; }
    public Archer_BattleState ArcherBattleState { get; set; }
    [Header("弓箭手细节")]
    [SerializeField][cn("箭矢预制体")] private GameObject arrowPrefab;
    [SerializeField][cn("发射点")] private Transform arrowStartPoint;
    [SerializeField][cn("箭矢速度")] private float arrowSpeed =10;
    [cn("箭矢强力击退")] public bool HearyHitback;//击退数值在实体血量的TakeDamage手动更改
    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        ArcherBattleState = new Archer_BattleState(this, stateMachine, "battle");
        battleState = ArcherBattleState;

        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }
    public override void SpecialAttack()//射箭
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().SetupArrow(arrowSpeed * faceDirection,combat);
        Destroy(arrow, 8);
    }
}
