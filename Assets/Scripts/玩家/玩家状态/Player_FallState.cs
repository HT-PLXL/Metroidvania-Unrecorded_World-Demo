using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.¼ì²âµ½µØÃæ)
            stateMachine.ChangeState(player.idleState);
        if (player.¼ì²âµ½Ç½±Ú)
            stateMachine.ChangeState(player.Ç½±Ú»¬¶¯×´Ì¬);

    }
}
