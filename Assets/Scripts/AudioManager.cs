using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private static float sfxGlobalVolume = 0.6f;  // 默认

    [SerializeField][cn("音频库")] private AudioDatabaseSO audioDatabase;
    [SerializeField][cn("背景音乐")] private AudioSource bgm;
    [SerializeField][cn("音效")] private AudioSource sfx;
    [SerializeField][cn("混音器")] private AudioMixer audioMixer;// 引用MasterMixer
    [SerializeField][cn("应该播放bgm?")] private bool bgmShouldPlay;
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private float mixerMultiplier = 25f;
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

        // 加载保存的SFX全局音量
        sfxGlobalVolume = PlayerPrefs.GetFloat("SFXGlobalVolume", 0.6f);
        // 加载保存的 BGM 音量（线性值）并应用到 Mixer
        float savedBgmLinear = PlayerPrefs.GetFloat("BGMVolume", 0.6f);
        SetBGMVolumeLinear(savedBgmLinear);

        // 确保bgm的volume始终为1，因为音量完全由Mixer控制
        if (bgm != null)
            bgm.volume = 1f;
        if (sfx != null)
            sfx.volume = 1f;  // sfx.volume 将动态计算，但这里设默认值
    }
    private void Start()
    {
        // 根据当前场景自动播放背景音乐
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "主菜单")
            //PlayBGM("主菜单音乐列表");
            return;
        else
            PlayBGM("混搭音乐组");
    }
    private void Update()
    {
        if (bgm.isPlaying == false && bgmShouldPlay && !IsApplicationPaused())
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
    public static void SetSFXGlobalVolume(float value)
    {
        sfxGlobalVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFXGlobalVolume", sfxGlobalVolume);
    }
    /// <summary>
    /// 自定义，用于设定AudioSource组件参数并播放音频，与声源距离有关
    /// </summary>
    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceCanHearSound = 15f)
    {
        if (string.IsNullOrEmpty(soundName))//null或空字符串-不播放不提醒
            return ;
        if (player == null)
            player = Player.instance.transform;

        var data = audioDatabase.Get(soundName);
        if (data == null)
        {
            Debug.Log("找不到音效 - " + soundName);
            return;
        }

        var clip = data.GetRandomClip();//随机音频
        if (clip == null)
            return;

        float maxVolume = data.volume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceCanHearSound));//获得百分比
        float distanceVolume = t * t; // 距离衰减系数

        sfxSource.pitch = Random.Range(0.9f, 1.1f);//随机音高
        // 最终音量 = 音效原始音量 * 全局SFX系数 * 距离衰减系数
        sfxSource.volume = maxVolume * sfxGlobalVolume * distanceVolume;
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
        sfx.volume = data.volume * sfxGlobalVolume; ;//音量
        sfx.PlayOneShot(clip);
    }
    public void StopBGM()
    {
        bgmShouldPlay = false;

        //StartCoroutine(FadeVolumeCo(bgm, 0, 1));

        if (currentBgmCo != null)
            StopCoroutine(currentBgmCo);
        currentBgmCo = StartCoroutine(FadeBGMVolumeCo(0f, 1f));
    }
    public void PlayBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName)
            return;

        // 如果没有正在播放的音乐，直接播放并设置音量（避免淡入带来的音量突变）
        if (!bgm.isPlaying)
        {
            AudioClipData data = audioDatabase.Get(musicGroup);
            if (data == null || data.clips.Count == 0)
            {
                Debug.LogWarning("找不到音乐组: " + musicGroup);
                return;
            }
            AudioClip nextMusic = data.GetRandomClip();
            if (nextMusic == null) return;

            lastMusic = nextMusic;
            currentBgmGroupName = musicGroup;
            bgm.clip = nextMusic;
            bgm.Play();

            float userVolume = PlayerPrefs.GetFloat("BGMVolume", 0.6f);
            SetBGMVolumeLinear(userVolume);  // 瞬间设置到正确音量
            return;
        }

        // 已有音乐在播放，使用正常的淡入淡出切换
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
        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("没找到音乐组，或音乐组音频数量为0" + musicGroup);
            yield break;
        }

        AudioClip nextMusic = data.GetRandomClip();
        if (data.clips.Count > 1)//不止一首时不重复播放
        {
            while (nextMusic == lastMusic)
                nextMusic = data.GetRandomClip();
        }

        // 如果当前有正在播放的音乐，淡出到0
        if (bgm.isPlaying)
            yield return FadeBGMVolumeCo(0f, 1f);//淡化音量，延迟至淡化结束后再执行后续语句

        lastMusic = nextMusic;
        bgm.clip = nextMusic;
        bgm.Play();

        float userVolume = PlayerPrefs.GetFloat("BGMVolume", 0.6f);
        //使用yield return启动的协程是子协程，父协程暂停时会一并暂停
        yield return FadeBGMVolumeCo(userVolume, 1f);//淡化音量
    }
    /// <summary>
    /// 自定义，平滑淡化音量的协程，用于在两首bgm间平滑过渡
    /// </summary>
    /// /// <param name="targetLinear">目标线性音量（0-1）</param>
    private IEnumerator FadeBGMVolumeCo(float targetLinear, float duration)
    {
        float startLinear = GetCurrentBGMVolumeLinear();
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float linear = Mathf.Lerp(startLinear, targetLinear, t);
            SetBGMVolumeLinear(linear);
            yield return null;
        }

        SetBGMVolumeLinear(targetLinear);
    }
    /// <summary>
    /// 获取当前BGM音量线性值（从Mixer读取dB转换）
    /// </summary>
    private float GetCurrentBGMVolumeLinear()
    {
        bool success = audioMixer.GetFloat(bgmVolumeParam, out float dB);
        if (!success)
            return 0.6f; // 默认值

        // dB = log10(linear) * mixerMultiplier  => linear = 10^(dB / mixerMultiplier)
        return Mathf.Pow(10f, dB / mixerMultiplier);
    }

    /// <summary>
    /// 设置BGM音量线性值
    /// </summary>
    private void SetBGMVolumeLinear(float linear)
    {
        float safeLinear = Mathf.Max(linear, 0.0001f);
        float dB = Mathf.Log10(safeLinear) * mixerMultiplier;
        audioMixer.SetFloat(bgmVolumeParam, dB);
    }

    // 提供一个公开方法供外部直接设置BGM音量（如果UI需要直接调）
    public void SetBGMVolume(float linear)
    {
        SetBGMVolumeLinear(linear);
    }
    public static float GetSFXGlobalVolume() => sfxGlobalVolume;



    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            PauseBGM();// 应用失去焦点（切出去），暂停BGM
        else
            ResumeBGM();// 应用获得焦点（切回来），恢复BGM
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            PauseBGM();// 应用被暂停（切出去）
        else
            ResumeBGM();// 应用恢复
    }
    public void PauseBGM()
    {
        if (bgm != null && bgm.isPlaying)
            bgm.Pause();  // 暂停，保留播放进度
    }
    public void ResumeBGM()
    {
        if (bgm != null && bgmShouldPlay && !bgm.isPlaying)
            bgm.UnPause();  // 从暂停处继续播放
    }
    // 辅助方法判断应用是否在后台
    private bool IsApplicationPaused()
    {
        return !Application.isFocused;
    }
}

