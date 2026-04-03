public class Enemy_GroundState : EnemyState
{
    public Enemy_GroundState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Update()
    {
        base.Update();

        if (enemy.DetectPlayer() == true)
            stateMachine.ChangeState(enemy.battleState);
        //DetectPlayer()返回的是RaycastHit2D 类型的变量hit
        //它是 Unity 为 2D 物理系统提供的结构体，用于存储射线检测的命中信息，包含碰撞体（collider）、碰撞点（point）、碰撞距离（distance）等关键数据
        //在 C# 中，RaycastHit2D结构体存在隐式布尔转换：
        //当 RaycastHit2D 的 collider 属性不为null时，会被视为true
        //当 collider 为null时，会被视为false
    }
}
