using UnityEngine;

public class Player_AnimationTrigger : Entity_AnimationTrigger
{
    private Player player;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
    }
    private void ThrowSword()//÷¿Ω£
    {
        player.skillManager.throwSword.ThrowSword();
    }
}
