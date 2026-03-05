using UnityEngine;

public class Player_AttackState : PlayerState
{
    private float speedOnAttackingTimer;
    private int comboIndex = 1;
    private int maxComboCount = 3;
    private int attackDirection;
    private float lastAttackTime;
    private bool IsComboSuccessful;
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(maxComboCount != player.attackVelocity.Length)
            maxComboCount = player.attackVelocity.Length;
        //限制/确定数组和连招总数是否一致，没啥用哈哈
    }
    public override void Enter()
    {
        base.Enter();
        IsComboSuccessful = false;
        ResetComboIndex();
        SetAttackSpeed();

        attackDirection = player.moveInput.x != 0 ? (((int)player.moveInput.x)): player.faceDirection;

        anim.SetInteger("普通攻击索引", comboIndex);

        CreateAttackSpeed();
    }


    public override void Update()
    {
        base.Update();

        HandleSpeedOnAttacking();

        if (input.Player.普通攻击.WasPressedThisFrame())
            IsComboSuccessful = true;

        if (triggerOver)
            HandleStateExit();
    }
    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastAttackTime = Time.time;//记录当前时间
    }
    private void HandleStateExit()
    {
        if (IsComboSuccessful)
        {
            anim.SetBool(animBoolName, false);//设置当前帧动画师界面的参数为假
            player.DelayEnterAttackState();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }
    private void HandleSpeedOnAttacking()
    {
        speedOnAttackingTimer -= Time.deltaTime;
        if (speedOnAttackingTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }
    private void CreateAttackSpeed()
    {
        Vector2 speedOnAttacking = player.attackVelocity[comboIndex - 1];
        speedOnAttackingTimer = player.attackSpeedDuration;
        player.SetVelocity(speedOnAttacking.x * attackDirection, speedOnAttacking.y);
        //这里的攻击方向改变速度方向，使得进入攻击状态时获得一个翻转的机会（player.HandleFlip）
    }
    private void ResetComboIndex()
    {
        if (comboIndex > maxComboCount)
            comboIndex = 1;//动画师界面：参数-普通攻击索引从1开始
        if (Time.time > lastAttackTime + player.comboResetTime)
            comboIndex = 1;
    }
}
