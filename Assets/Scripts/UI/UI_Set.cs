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
    [SerializeField][cn("填入混音器选择暴露出的参数")] private string bgmParameter;
    [Header("音效音量设置")]
    [SerializeField][cn("音效滑动条")] private Slider sfxSlider;
    [SerializeField][cn("填入混音器选择暴露出的参数")] private string sfxParameter;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        hpBarToggle.onValueChanged.AddListener(OnToggleMiniHpBar);
    }
    private void OnToggleMiniHpBar(bool isOpen)
    {
        player.hp.EnableHpBar(isOpen);
    }
    public void ToggleToMainMenuButton()
    {
        GameSceneManager.instance.ChangeScene("主菜单", RespawnType.None);
    }
    //Unity 内置机制：Slider滑动条组件最下方有个 On Value Changed模块
    //每次值变动都会把滑动条当前的Value值作为参数传入选定的方法，即下面的方法BGMSliderValue(float value)
    public void BGMSliderValue(float value)//在组件中设置最小值为0.0001
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
    }
    public void SFXSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParameter, newValue);
    }
    /// <summary>
    /// 自定义，用于被调用，解决音量设置需要设置界面激活，OnEnable才会执行的问题
    /// </summary>
    public void LoadVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);
    }
    private void OnEnable()
    {
        //PlayerPrefs 是 Unity 提供的本地数据持久化存储系统，能存int，float，string
        // 从 PlayerPrefs 读取音量值（如果不存在则默认 0.6f）
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);
    }

    private void OnDisable()
    {
        // 保存当前音量值到 PlayerPrefs
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }
}
