using UnityEngine;

public class CheckPoint : MonoBehaviour, ISaveable
{
    private CheckPoint[] allCheckPoints;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        allCheckPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);

    }
    public void ActivateCheckPoint(bool activate)
    {
        anim.SetBool("active", activate);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var point in allCheckPoints)
            point.ActivateCheckPoint(false);
        SaveManager.instance.GetGameData().saveCheckPoint = transform.position;
        ActivateCheckPoint(true);
    }

    public void loadData(GameData data)
    {
        bool active = data.saveCheckPoint == transform.position;// 判断保存的位置是否等于某个检查点的位置
        ActivateCheckPoint(active);
        if (active)
            Player.Instance.TeleportPlayer(transform.position);
    }

    public void SavaData(ref GameData data)
    {

    }
}
