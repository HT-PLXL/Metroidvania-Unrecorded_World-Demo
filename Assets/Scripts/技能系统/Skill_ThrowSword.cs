using UnityEngine;
//挂载在技能容器上，下方声明的参数用来在技能容器修改对应技能实体脚本声明的参数的值
public class Skill_ThrowSword : Skill_Base
{
    private SkillObject_Sword currentSword;
    [SerializeField][cn("飞剑预制体")] private GameObject SwordPrefab;
    [SerializeField][cn("投掷力度")] private float throwPower = 26;//投掷力度
    [SerializeField][cn("重力缩放倍率")] private float gravityScale = 3.5f;//重力缩放倍率
    [Header("弹道")]
    [SerializeField][cn("小圆点预制体")] private GameObject dot;//小圆点
    [SerializeField][cn("小圆点数量")] private int dotNumber = 20;//小圆点数量
    [SerializeField][cn("间隔")] private float dotInterval = 0.05f;//间隔
    [Header("噬魂影刃")]
    [SerializeField][cn("噬魂影刃预制体")] private GameObject swordPiercePrefab;
    [cn("可穿透数量")]public int canPierceNumber = 2;
    [Header("裂空旋刃")]
    [SerializeField][cn("裂空旋刃预制体")] private GameObject spinSwordPrefab;
    [cn("最大存在时间")] public float maxDuration=6;
    [cn("最大攻击时间")] public float maxAttackDuration=2;
    [cn("每秒攻击次数")] public float attackPerSecond=3;//每秒攻击次数
    [cn("最大飞行距离")] public float maxFlyDistance = 20;
    [Header("回旋镖大师")]
    [SerializeField][cn("弹射旋刃预制体")] private GameObject bounceSwordPrefab;
    [cn("弹射速度")] public float bounceSpeed = 15;
    [cn("弹射次数")]public int bounceNumber = 5;
    [cn("检测范围")] public float checkRadius;

    private Transform[] allDots;
    private Vector2 mouseDirection;
    protected override void Awake()
    {
        base.Awake();
        allDots = CreateDots();
    }
    public override bool CanUseSkill()
    {
        if (currentSword != null)
        {
            currentSword.ShouldComeBack();
            return false;
        }
        return base.CanUseSkill();
    }
    public void ThrowSword()//动画节点触发的方法，掷剑逻辑起点
    {
        GameObject UsedSwordPrefab = GetSwordPrefab();//选择技能升级种类
        GameObject newSword = Instantiate(UsedSwordPrefab, allDots[1].position, Quaternion.identity);//生成技能升级对应实体
        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetUpSword(this, mouseDirection * throwPower, IsPhysicalDamage);//传入各种掷剑升级的信息用于技能实体脚本引用覆盖默认值，以及传入初速度用于掷剑
        setSkillOnCooldown();
    }
    private GameObject GetSwordPrefab()//选择技能升级种类，拓展升级需要来更新
    {
        if (Unlocked(SkillUpgradeType.掷剑))
            return SwordPrefab;
        if (Unlocked(SkillUpgradeType.噬魂影刃))
            return swordPiercePrefab;
        if (Unlocked(SkillUpgradeType.裂空旋刃))
            return spinSwordPrefab;
        if (Unlocked(SkillUpgradeType.灵舞旋刃))
            return bounceSwordPrefab;
        Debug.Log("无效的剑技升级喵！");
        return null;
    }
    public void GetTrajectory(Vector2 direction)//计算轨迹
    {
        for (int i = 0; i < allDots.Length; i++)
        {
            allDots[i].position = GetTrajectoryPoint(direction, i * dotInterval);//i * dotInterval模拟间隔的t
        }
    }
    public Vector2 GetTrajectoryPoint(Vector2 direction, float t)//斜抛运动方法，输入t得到位置
    {
        //direction 是单位向量，鼠标方向
        float VelocityX = direction.x * throwPower;
        float VelocityY = direction.y * throwPower;
        float positionX = VelocityX * t;
        float positionY = VelocityY * t + 0.5f * Physics2D.gravity.y * gravityScale * t * t;//Physics2D.gravity：g,默认为负，vector2类型
        Vector2 dotPosition = new Vector2(positionX, positionY);
        Vector2 playerPosition = transform.root.position;
        return playerPosition + dotPosition;
    }
    public void GetDotDirection(Vector2 direction) => mouseDirection = direction;
    public void EnableDots(bool enable)
    {
        foreach (Transform transform in allDots)
            transform.gameObject.SetActive(enable);
    }
    private Transform[] CreateDots()
    {
        Transform[] newDots = new Transform[dotNumber];//dotNumber此处可当做长度
        for (int i = 0; i < dotNumber; i++)
        {
            newDots[i] = Instantiate(dot, transform.position, Quaternion.identity, transform).transform;//旋转后面的transform是父对象，即生成的点作为子对象
            newDots[i].gameObject.SetActive(false);
        }
        return newDots;
    }
}