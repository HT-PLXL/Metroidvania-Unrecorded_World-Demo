using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
//该类挂载于设置UI
public class UI_Set : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle hpBarToggle;
    [SerializeField] private AudioMixer audioMixer;//序列化选择的混音器（创建在Assets-Settings中）
    [SerializeField] private float mixerMultiplier = 25;
    [Header("背景音乐音量设置")]
    [SerializeField][cn("背景音乐滑动条")] private Slider bgmSlider;
    [SerializeField][cn("填入混音器选择暴露出的参数")] private string bgmParameter = "BGMVolume";
    [Header("音效音量设置")]
    [SerializeField][cn("音效滑动条")] private Slider sfxSlider;
    private const string hpBarKey = "HpBarEnabled";
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        hpBarToggle.onValueChanged.AddListener(OnToggleMiniHpBar);
        hpBarToggle.onValueChanged.AddListener(SaveHpBarState);
    }
    private void OnToggleMiniHpBar(bool isOpen)
    {
        // 确保 player 和 player.hp 存在
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player != null && player.hp != null)
            player.hp.EnableHpBar(isOpen);
    }
    private void SaveHpBarState(bool isOpen)
    {
        PlayerPrefs.SetInt(hpBarKey, isOpen ? 1 : 0);
    }
    public void ToggleToMainMenuButton()
    {
        GameSceneManager.instance.ChangeScene("主菜单", RespawnType.None);
    }
    //Unity 内置机制：Slider滑动条组件最下方有个 On Value Changed模块
    //每次值变动都会把滑动条当前的Value值作为参数传入选定的方法，即下面的方法BGMSliderValue(float value)
    public void BGMSliderValue(float value)//在组件中设置最小值为0.0001
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * mixerMultiplier;
        audioMixer.SetFloat(bgmParameter, dB);
        // 保存到PlayerPrefs
        PlayerPrefs.SetFloat(bgmParameter, value);
    }
    public void SFXSliderValue(float value)
    {
        // 直接设置AudioManager的全局SFX音量
        AudioManager.SetSFXGlobalVolume(value);
    }
    private void OnEnable()
    {
        // 加载BGM音量（线性值）并应用到Mixer
        float savedBgm = PlayerPrefs.GetFloat(bgmParameter, 0.6f);
        bgmSlider.value = savedBgm;
        BGMSliderValue(savedBgm);

        // 加载SFX音量并应用到AudioManager
        float savedSfx = PlayerPrefs.GetFloat("SFXGlobalVolume", 0.6f);
        sfxSlider.value = savedSfx;
        AudioManager.SetSFXGlobalVolume(savedSfx);

        // 加载血条开关状态
        bool hpBarEnabled = PlayerPrefs.GetInt(hpBarKey, 1) == 1; // 默认开启(1)
        hpBarToggle.SetIsOnWithoutNotify(hpBarEnabled); // 不触发回调，避免重复调用
        OnToggleMiniHpBar(hpBarEnabled); // 手动应用设置
    }

    private void OnDisable()
    {
        // 保存当前值（BGM已在BGMSliderValue中保存，但为了保险再保存一次）
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
        PlayerPrefs.SetFloat("SFXGlobalVolume", sfxSlider.value);
    }
}
