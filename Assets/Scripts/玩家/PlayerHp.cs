using UnityEngine;

public class PlayerHp : EntityHp, ISaveable, ILoadDataCompletion
{
    private Player player;
    private float pendingHp; // 暂存待设置的血量
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

        if (player.ui == null)
        {
            Debug.Log("player.ui == null");
            player.ui = FindFirstObjectByType<UI>();
        }    
        player.ui.OpenDeathScreenUI();
    }

    public void SavaData(ref GameData data)
    {
        data.playerCurrentHp = currentHp;
    }

    public void loadData(GameData data)
    {
        // 先暂存，等全部数据（装备、属性）加载完再应用
        pendingHp = data.playerCurrentHp;
    }

    public void OnAllDataLoaded()
    {
        float maxHp = entityStats != null ? entityStats.GetMaxHp() : pendingHp;
        // 新存档或异常血量为0 → 设为满血
        if (pendingHp <= 0f)
            currentHp = maxHp;
        else
            currentHp = Mathf.Clamp(pendingHp, 0, maxHp);
        TriggerHpUpdate(); // 使用基类提供的方法触发事件
    }
}
