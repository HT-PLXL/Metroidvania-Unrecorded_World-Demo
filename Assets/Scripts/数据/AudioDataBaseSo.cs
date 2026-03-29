using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="音频/音频列表")]
public class AudioDatabaseSO : ScriptableObject
{
    public List<AudioClipData> player;
    public List<AudioClipData> uiAudio;

    private Dictionary<string, AudioClipData> clipCollection;//音频集合（字典）<audioName组名，音频列表>

    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();
        AddToCollection(player);
        AddToCollection(uiAudio);
    }

    public AudioClipData Get(string groupName)
    {
        return clipCollection.TryGetValue(groupName, out AudioClipData data) ? data : null;
    }

    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var data in listToAdd)
        {
            if (data != null && clipCollection.ContainsKey(data.audioName) == false)
            {
                clipCollection.Add(data.audioName, data);
            }
        }
    }
}

[System.Serializable]
public class AudioClipData
{
    public string audioName;
    public List<AudioClip> clips = new List<AudioClip>();//内置的音频片段类
    [Range(0f, 1f)] public float volume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Count == 0)
            return null;

        return clips[Random.Range(0, clips.Count)];//0 和 clips.Count是int类型，所以随机的数也是整数
    }
}