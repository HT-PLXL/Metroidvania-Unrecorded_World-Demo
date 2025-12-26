using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        anim.enabled = false;

        rb.gravityScale = 12;//增大重力
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);
        enemy.GetComponent<Collider2D>().enabled = false;
        //Vector2 击飞方向 = new Vector2(1, 1);
        //float 击飞力量 = 5f;
        //rb.AddForce(击飞方向 * 击飞力量, ForceMode2D.Impulse);
        //这里写的不好，失去速度或者力后会垂直下落，有时间再找原因吧
        stateMachine.关闭状态机();
    }
}
