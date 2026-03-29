using UnityEngine;

public class CheckPoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkPointId;
    [SerializeField] private Transform respawnPoint;
    public bool isActive { get; private set; }
    private Animator anim;
    private AudioSource fireAudioSource;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        fireAudioSource = GetComponent<AudioSource>();
    }
    public Vector3 getPosition() => respawnPoint == null ? transform.position : respawnPoint.position;
    public string GetCheckPointId() =>checkPointId;
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkPointId))//仅当从未初始化时才会自动生成一个id
        {
            checkPointId = System.Guid.NewGuid().ToString();
        }
#endif
    }
    public void ActivateCheckPoint(bool activate)
    {
        isActive = activate;
        anim.SetBool("active", activate);
        if (isActive && fireAudioSource.isPlaying ==false)
            fireAudioSource.Play();
        if(isActive == false)
            fireAudioSource.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateCheckPoint(true);//激活当前触碰的检查点
    }

    public void loadData(GameData data)
    {
        bool active = data.unlockedCheckPoints.TryGetValue(checkPointId, out active);
        ActivateCheckPoint(active);//获取到字典中有这个实例的话就激活
    }

    public void SavaData(ref GameData data)
    {
        if (isActive == false)//如果这个实例没有被激活直接返回
            return;
        //如果没有字典里没有匹配到这个id，说明是第一次激活，保存数据
        if (data.unlockedCheckPoints.ContainsKey(checkPointId) == false)
            data.unlockedCheckPoints.Add(checkPointId, true);
    }
}
