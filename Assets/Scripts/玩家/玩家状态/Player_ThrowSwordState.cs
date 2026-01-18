using UnityEngine;

public class Player_ThrowSwordState : PlayerState
{
    private Camera mainCamera;
    public Player_ThrowSwordState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Update()
    {
        base.Update();
        Vector2 mouseDirection = MouseDirection();
        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(mouseDirection.x);
        skillManager.throwSword.GetTrajectory(mouseDirection);

        if (input.Player.普通攻击.WasPerformedThisFrame())//掷出
        {
            anim.SetBool("throwSwordPerformed", true);
            skillManager.throwSword.EnableDots(false);//掷出隐藏轨迹点
            skillManager.throwSword.GetDotDirection(mouseDirection);
        }
        if (input.Player.掷剑.WasReleasedThisFrame() || triggerOver)//瞄准阶段松手退出以及投出阶段触发节点退出(在最后一帧上)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Enter()
    {
        base.Enter();
        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
        skillManager.throwSword.EnableDots(true);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool("throwSwordPerformed", false);
        skillManager.throwSword.EnableDots(false);
    }
    private Vector2 MouseDirection()//获取鼠标位置方向
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);
        Vector2 direction = worldMousePosition - playerPosition;
        return direction.normalized;//归一化，返回新的方向向量
    }
}
