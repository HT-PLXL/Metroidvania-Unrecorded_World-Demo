using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.检测到地面)
            stateMachine.ChangeState(player.idleState);
        if (player.检测到墙壁)
            stateMachine.ChangeState(player.wallSlideState);

    }
}
