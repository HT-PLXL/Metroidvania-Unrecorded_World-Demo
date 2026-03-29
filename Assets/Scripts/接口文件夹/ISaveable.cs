using UnityEngine;
//“需要保存”接口
//各处需要保存的数据经过接口提供的方法loadData进行保存，再通过FileDataHandler类，文件数据管理器的方法将数据转化为.json文件
//读档时将.json转化回数据，再通过接口的loadData覆盖各处的数据
public interface ISaveable
{
    //注意，子类继承的SavaData在使用GameData的可序列化字典前需要先清空
    public void SavaData(ref GameData data);//ref保证传入的是data的引用，修改会影响原数据
    public void loadData(GameData data);
}