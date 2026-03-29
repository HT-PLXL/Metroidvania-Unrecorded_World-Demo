using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField][cn("音频库")] private AudioDatabaseSO audioDatabase;
    [SerializeField][cn("背景音乐")] private AudioSource bgm;
    [SerializeField][cn("音效")] private AudioSource sfx;
    private Transform player;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 自定义，用于设定AudioSource组件参数并播放音频
    /// </summary>
    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceCanHearSound = 15f)
    {
        if (player == null)
            player = Player.instance.transform;
        var data = audioDatabase.Get(soundName);
        if (data == null)
        {
            Debug.Log("找不到音效 - " + soundName);
            return;
        }

        var clip = data.GetRandomClip();//随机音频
        if (clip == null) return;

        float maxVolume = data.volume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceCanHearSound));//获得百分比

        sfxSource.pitch = Random.Range(0.9f, 1.1f);//随机音高
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t);//设为音量的t百分比
        sfxSource.PlayOneShot(clip);//播放一次
    }
    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDatabase.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        sfx.pitch = Random.Range(.95f, 1.1f);//随机音高
        sfx.volume = data.volume;//音量
        sfx.PlayOneShot(clip);
    }
}
