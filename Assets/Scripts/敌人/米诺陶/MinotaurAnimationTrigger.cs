public class MinotaurAnimationTrigger : EnemyAnimationTrigger
{
    private Minotaur minotaur;

    protected override void Awake()
    {
        base.Awake();
        minotaur = GetComponentInParent<Minotaur>();
    }

    //撼地伤害
    public void StompDamageEvent()
    {
        minotaur.CreateSmoke();
        minotaur.stompState.PerformStompDamage();
    }

    //冲撞伤害
    public void ChargeDamageEvent()
    {
        minotaur.chargeState.PerformChargeDamage();
    }

    //旋转横扫伤害
    public void SpinDamageEvent()
    {
        minotaur.spinState.PerformSpinDamage();
    }
    //劈砍伤害
    public void SlashDamageEvent()
    {
        minotaur.slashState?.PerformSlashDamage();
    }
    //普通攻击伤害
    public void BasicAttackDamageEvent()
    {
        minotaur.basicAttackState?.PerformBasicDamage();
    }
}