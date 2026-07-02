using UnityEngine;
using System.Threading.Tasks;

public class Player_AirComboState : PlayerState
{
    private int comboIndex = 1;
    private int maxComboCount = 3;// 最多连击段数
    private float lastAttackTime;
    private bool isComboSuccessful;
    private float originalGravity;
    private float StopAirAttackVelocityTimer;
    private int attackDirection;

    public Player_AirComboState(Player player, StateMachine stateMachine, string animBoolName): base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isComboSuccessful = false;
        ResetComboIndex();
        player.SetVelocity(0,0);
        originalGravity = rb.gravityScale;
        rb.gravityScale = originalGravity * 0.2f;

        SetAttackSpeed();
        attackDirection = player.moveInput.x != 0 ? (((int)player.moveInput.x)) : player.faceDirection;
        anim.SetInteger("AirComboIndex", comboIndex);

        _ = ApplyAirAttackVelocity();
    }

    public override void Update()
    {
        base.Update();
        StopAirAttackVelocity();

        if (input.Player.普通攻击.WasPressedThisFrame())
            isComboSuccessful = true;

        //if (player.moveInput.x != 0)// 水平移动（使用空中移动倍率）
        //    player.SetVelocity(player.moveInput.x * (player.moveSpeed * player.airMoveMultiplier), rb.linearVelocity.y);

        if (player.isDetectGround)//落地
            stateMachine.ChangeState(player.idleState);

        if (triggerOver)// 动画节点触发结束
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastAttackTime = Time.time;
        rb.gravityScale = originalGravity;   // 恢复重力
    }

    private void HandleStateExit()
    {
        if (isComboSuccessful && comboIndex < maxComboCount)
        {
            // 继续下一段连击
            anim.SetBool(animBoolName, false);
            player.DelayEnterAirComboState();   // 类似地面连招的延迟进入方法
        }
        else
        {
            if (comboIndex >= maxComboCount)//如果本次是最后一段攻击，连击用完 == true
                player.airComboExhausted = true;

            stateMachine.ChangeState(player.fallState);
        }
    }

    private void ResetComboIndex()//判断并重置连招索引
    {
        if (comboIndex > maxComboCount)
            comboIndex = 1;
        if (Time.time > lastAttackTime + player.airComboResetTime)
            comboIndex = 1;
    }

    private void StopAirAttackVelocity()
    {
        StopAirAttackVelocityTimer -= Time.deltaTime;
        if (StopAirAttackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }
    private async Task ApplyAirAttackVelocity()
    {
        Vector2 speedOnAttacking = player.velocityOnAirAttack[comboIndex - 1];
        if (comboIndex == 2)//对第二段普攻使用
            await Task.Delay(player.airAttackVelocityDelay);//攻击时突进速度的应用延迟
        StopAirAttackVelocityTimer = player.airAttackSpeedDuration;
        player.SetVelocity(speedOnAttacking.x * attackDirection, speedOnAttacking.y);
        //这里的攻击方向改变速度方向，使得进入攻击状态时获得一个翻转的机会（player.HandleFlip）
    }

}