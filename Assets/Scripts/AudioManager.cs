using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField][cn("音频库")] private AudioDatabaseSO audioDatabase;
    [SerializeField][cn("背景音乐")] private AudioSource bgm;
    [SerializeField][cn("音效")] private AudioSource sfx;
    [SerializeField][cn("应该播放bgm?")] private bool bgmShouldPlay;
    private Transform player;
    private AudioClip lastMusic;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    private void Awake()
    {
        //单例模式
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (bgm.isPlaying == false && bgmShouldPlay)
        {
            //没有播放音乐 且 应该播放音乐 且 当前音乐组不为空则切换
            if (string.IsNullOrEmpty(currentBgmGroupName) == false)
                SwitchBGM(currentBgmGroupName);
        }

        if (bgm.isPlaying && bgmShouldPlay == false)
        {
            StopBGM();
        }
    }
    /// <summary>
    /// 自定义，用于设定AudioSource组件参数并播放音频，与声源距离有关
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
    /// <summary>
    /// 自定义，播放全局音效，如点击按钮，与声源距离无关
    /// </summary>
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
    public void StopBGM()
    {
        bgmShouldPlay = false;

        StartCoroutine(FadeVolumeCo(bgm, 0, 1));

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
    }
    public void PlayBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName)
            return;

        SwitchBGM(musicGroup);
    }
    public void SwitchBGM(string musicGroup)
    {
        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }
    /// <summary>
    /// 自定义，切换音乐的协程，会短暂淡化当前bgm和新bgm的音量进行过渡
    /// </summary>
    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDatabase.Get(musicGroup);
        AudioClip nextMusic = data.GetRandomClip();

        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("没找到音乐组，或音乐组音频数量为0" + musicGroup);
            yield break;
        }

        if (data.clips.Count > 1)//不止一首时不重复播放
        {
            while (nextMusic == lastMusic)
                nextMusic = data.GetRandomClip();
        }

        if (bgm.isPlaying)
            yield return FadeVolumeCo(bgm, 0, 1f);//淡化音量，延迟至淡化结束后再执行后续语句

        lastMusic = nextMusic;
        bgm.clip = nextMusic;
        bgm.volume = 0;
        bgm.Play();

        //使用yield return启动的协程是子协程，父协程暂停时会一并暂停
        yield return FadeVolumeCo(bgm, data.volume, 1f);//淡化音量
    }
    /// <summary>
    /// 自定义，平滑淡化音量的协程，用于在两首bgm间平滑过渡
    /// </summary>
    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }
}
