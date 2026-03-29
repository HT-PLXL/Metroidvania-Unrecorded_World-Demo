using UnityEngine;

public class PlayerHp : EntityHp
{
    private Player player;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            Die();
    }
    protected override void Die()
    {
        base.Die();

        player.ui.OpenDeathScreenUI();
    }
}
