using UnityEngine;

public class Player_IdleState : Player_GroundState
{

    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    //子类 Player_IdleState 的构造函数接收player, stateMachine, stateName 三个参数；
    //通过 : base(player，stateMachine, stateName)，将这三个参数传递给父类 EntityState 的构造函数
    //确保父类中player和 stateMachine 和 stateName 成员变量被正确初始化。
    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, rb.linearVelocity.y);
    }
    public override void Update()//由于初始是idle状态，且是本脚本的Player_IdleState类
                                 //所以Player.cs执行的Update中stateMachine.UpdataActiveState（）
                                 //调用状态机currentState.Update()再调用这里的Update
    {                            //而这里的base会再次向父级Player_GroundState传递
        base.Update();

        if (player.moveInput.x == player.面对方向 && player.检测到墙壁)
            return;
        if (player.moveInput.x !=0)
            stateMachine.ChangeState(player.moveState);

    }




}
