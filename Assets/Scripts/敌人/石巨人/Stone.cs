using System;
using System.Collections;
using UnityEngine;
//Boos——石巨人
public class Stone : Enemy, ICounter
{
    public bool CanBeCountered { get => canBeStunned; }
    public Stone_ActiveState activeState { get; set; }
    public bool active { get; set; }
    public Stone_AttackState attack { get; set; }
    public UI_BOOSHpBar boosHpBar { get; set; }
    public StoneHp stoneHp { get; set; }
    [Header("石巨人专属")]
    public int stage = 0;
    [Header("石刺技能")]
    [SerializeField][cn("石刺预制体")] private GameObject spikePrefab;
    [cn("石刺数量")] public int spikesNumber = 5;
    [cn("石刺移动速度")] public float spikeMoveSpeed;
    [cn("石刺存在时间")] public float spikeTime;
    [cn("石刺出现延迟乘数")] public float delayMultiplier;
    [Header("激光技能")]
    [SerializeField][cn("激光预制体")] private GameObject laserPrefab;
    [SerializeField][cn("激光生成位置")] private Transform CreatePosition;
    [cn("技能冷却")] public float skillCooldown;
    private GameObject laser;

    [Header("漂浮细节")]
    public bool shouldFloat;
    [SerializeField][cn("漂浮速度")] private float floatSpeed = 5f;
    [SerializeField][cn("漂浮范围")] private float floatRange = 0.1f;
    private Vector3 startPosition;
    public bool isTurning { get; set; }
    public float lastTimeUseSkill = -999;
    public bool attackOver { get; set; }
    public bool skillOver { get; set; }
    public bool isUsingSkill { get; set; }
    protected override void Awake()
    {
        base.Awake();
        //moveState = new Enemy_MoveState(this, stateMachine, "move");
        //stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        //deadState = new Enemy_DeadState(this, stateMachine, "dead");

        activeState = new Stone_ActiveState(this, stateMachine, "active");
        attack = new Stone_AttackState(this, stateMachine, "attack");
        battleState = new Stone_BattleState(this, stateMachine, "battle");
        //地面状态索敌会转入battleState状态，所以新角色需要给battle赋值，如battleState = ArcherBattleState;

        boosHpBar = FindAnyObjectByType<UI_BOOSHpBar>(FindObjectsInactive.Include);
        stoneHp = GetComponent<StoneHp>();

        startPosition = transform.position;
        shouldFloat = false;
        haveDeadAnim = true;
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(activeState);
    }
    protected override void Update()
    {
        base.Update();
        //Debug.Log(stateMachine.currentState);
        if (attackOver == true)
            anim.SetBool("attackOver", true);

        if (shouldFloat)
            HandleFloat();
    }
    public void HandleCounter()//实现接口
    {
        stateMachine.ChangeState(stunnedState);
    }

    public void UseSpike()
    {
        CreateSpikes();
    }
    public GameObject[] CreateSpikes()
    {
        GameObject[] newSpikes = new GameObject[spikesNumber];
        int direction = GetDirection();
        for (int i = 0; i < spikesNumber; i++)
        {
            Vector3 position = GetCreatePosition(direction, i);
            newSpikes[i] = Instantiate(spikePrefab, position, spikePrefab.transform.rotation);
            newSpikes[i].GetComponent<Stone_Spike>().SetSpike(this, i);
        }
        return newSpikes;
    }
    private int GetDirection()
    {
        int direction = MathF.Sign(transform.position.x - player.GetComponent<Player>().transform.position.x);
        if (direction == 0)
            direction = 1;
        return direction;
    }
    private Vector3 GetCreatePosition(int direction, int i)
    {
        Vector3 pos = transform.position;
        pos.x = pos.x - 1.5f * direction * i;
        pos.y = transform.position.y - 6.3f;
        return pos;
    }

    protected void HandleFloat()
    {
        float yOffect = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        Vector3 pos = transform.position;
        pos.y = startPosition.y + yOffect;
        transform.position = pos;
    }
    public void CreateLaser()
    {
        laser = Instantiate(laserPrefab, CreatePosition.position, Quaternion.identity);
        LaserBeam2D laserScript = laser.GetComponent<LaserBeam2D>();

        // 关键：将激光的 startPoint 设置为 CreatePosition
        laserScript.startPoint = CreatePosition;

        laserScript.SetLaser(this);
        lastTimeUseSkill = Time.time;
    }
    public override void AfterDie()
    {
        boosHpBar.gameObject.SetActive(false);
    }
}
