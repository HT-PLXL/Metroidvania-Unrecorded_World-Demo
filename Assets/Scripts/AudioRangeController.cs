using UnityEngine;
//该类挂载在对象后，会随距离修改AudioSource组件的音量大小
public class AudioRangeController : MonoBehaviour
{
    private AudioSource source;
    private Transform player;

    [SerializeField] private float minDistanceCanHearSound = 12;
    [SerializeField] private bool showGizmo;
    private float maxVolume;

    private void Start()
    {
        player = Player.instance.transform;
        source = GetComponent<AudioSource>();
        maxVolume = source.volume; // 原始最大音量（不带全局系数）
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceCanHearSound));
        float distanceVolume = t * t;
        // 最终音量 = 原始最大音量 * 全局SFX系数 * 距离衰减
        float targetVolume = maxVolume * AudioManager.GetSFXGlobalVolume() * distanceVolume;
        if (Mathf.Abs(source.volume - targetVolume) > 0.01f)
            source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime * 5);
        else
            source.volume = targetVolume;
    }
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minDistanceCanHearSound);
        }
    }
}
