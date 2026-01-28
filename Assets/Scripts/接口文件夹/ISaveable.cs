using UnityEngine;

public interface ISaveable
{
    public void loadData(GameData data);
    public void SavaData(ref GameData data);
}
