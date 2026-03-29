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
        maxVolume = source.volume;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceCanHearSound));
        //平滑插值
        float targetVolume = Mathf.Lerp(0, maxVolume, t * t);
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
